using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DataDynamics.PageFX.CodeModel;
using DataDynamics.PageFX.CodeModel.TypeSystem;
using DataDynamics.PageFX.FLI.ABC;
using DataDynamics.PageFX.FLI.IL;

namespace DataDynamics.PageFX.FLI
{
    //main part of generator - contains entry point to the generator.
    partial class AbcGenerator : IDisposable
    {
        #region Shared Members
        public static AbcFile ToAbcFile(IAssembly assembly)
        {
            using (var g = new AbcGenerator())
            {
                return g.Generate(assembly);
            }
        }

        public static void Save(IAssembly assembly, string path)
        {
            var f = ToAbcFile(assembly);
            f.Save(path);
        }

        public static void Save(IAssembly assembly, Stream output)
        {
            var f = ToAbcFile(assembly);
            f.Save(output);
        }
        #endregion

        #region IDisposable Members
        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }

        // Use C# destructor syntax for finalization code.
        ~AbcGenerator()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
        #endregion

        #region Fields
        IAssembly _assembly;
        IMethod _entryPoint;
        internal AbcFile _abc;
        
        //If not null indicates that we genearate swiff file.
        internal SwfCompiler sfc;

        public AbcGenMode Mode;
        #endregion

        #region Properties
        public IAssembly ApplicationAssembly
        {
            get { return _assembly; }
        }

        /// <summary>
        /// Indicates whether we are compiling swf file.
        /// </summary>
        public bool IsSwf
        {
            get { return sfc != null; }
        }

        /// <summary>
        /// Indicates whether we are compiling swc file.
        /// </summary>
        public bool IsSwc
        {
            get { return sfc != null && sfc.IsSwc; }
        }

        public bool IsMxApplication
        {
            get
            {
                if (sfc != null)
                    return sfc.IsFlexApplication;
                return false;
            }
        }

        public string RootNamespace
        {
            get 
            {
                if (sfc != null)
                {
                    string ns = sfc.RootNamespace;
                    if (ns != null) return ns;
                }
                return "";
            }
        }

        public AbcNamespace RootAbcNamespace
        {
            get { return _nsroot ?? (_nsroot = _abc.DefinePackage(RootNamespace)); }
        }
        private AbcNamespace _nsroot;
        #endregion

        #region Generate - Entry Point
        public AbcFile Generate(IAssembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

#if PERF
            int start = Environment.TickCount;
#endif

#if DEBUG
            DebugService.LogInfo("ABC Generator started");
            DebugService.LogSeparator();
            DebugService.DoCancel();
#endif
            _assembly = assembly;

            AssemblyIndex.Setup(assembly);
            Linker.Start(assembly);

            _abc = new AbcFile
                       {
                           AutoComplete = true,
                           ReduceSize = true,
                           generator = this,
                           SwfCompiler = sfc,
                           Assembly = assembly
                       };

            AssemblyTag.Instance(_assembly).AddAbc(_abc);

            if (sfc != null)
            {
                sfc.FrameApp = _abc;
                if (sfc.IsSwc)
                    Mode = AbcGenMode.Full;
            }

            _newAPI = new AbcCode(_abc);
            RegisterObjectFunctions();

            BuildApp();

            #region Finish Application
#if DEBUG
            DebugService.DoCancel();
#endif
			if (sfc != null)
			{
				sfc.FinishApplication();
			}

        	#endregion

            BuildRootTimeline();

            #region Finish Types
#if DEBUG
            DebugService.DoCancel();
#endif

            FinishTypes();
            #endregion

            #region Late Methods
#if DEBUG
            DebugService.DoCancel();
#endif

            _lateMethods.Finish();
            #endregion

            #region Scripts
#if DEBUG
            DebugService.DoCancel();
#endif
            BuildScripts();
            #endregion

            FinishMainScript();

            #region Finish ABC
#if DEBUG
            DebugService.DoCancel();
#endif

            _abc.Finish();
            #endregion

#if PERF
            Console.WriteLine("ABC.MultinameCount: {0}", _abc.Multinames.Count);
            Console.WriteLine("AbcGenerator Elapsed Time: {0}", Environment.TickCount - start);
#endif

            return _abc;
        }
        #endregion

        #region BuildApp
        void BuildApp()
        {
#if PERF
            int start = Environment.TickCount;
#endif
            switch (Mode)
            {
                case AbcGenMode.Default:
                    BuildAppDefault();
                    break;

                case AbcGenMode.Full:
                    BuildAssemblyTypes();
                    break;
            }

#if PERF
            Console.WriteLine("AbcGen.Build: {0}", Environment.TickCount - start);
#endif
        }

        void BuildAppDefault()
        {
            _entryPoint = _assembly.EntryPoint;
            if (_entryPoint != null)
                DefineMethod(_entryPoint);
            else
                BuildLibrary();

            BuildExposedAPI();
        }
        #endregion

        #region BuildExposedAPI
        void BuildExposedAPI()
        {
            foreach (var type in _assembly.Types)
            {
                if (type.IsTestFixture())
                    _testFixtures.Add(type);
                if (type.IsExposed())
                    DefineType(type);
            }
        }
        #endregion

        #region BuildLibrary
        void BuildLibrary()
        {
            if (IsMxApplication)
            {
                var type = sfc.TypeFlexApp;
                if (type == null)
                    throw new InvalidOperationException();
                DefineType(type);
                return;
            }

            if (IsSwf)
            {
				var type = _assembly.Types.FirstOrDefault(IsRootSprite);
                if (type != null)
                {
                    DefineType(type);
                    return;
                }
            }

            BuildAssemblyTypes();
        }

        void BuildAssemblyTypes()
        {
            var list = new List<IType>(_assembly.Types);
            foreach (var type in list)
            {
                if (GenericType.HasGenericParams(type)) continue;
                DefineType(type);
            }
        }
        #endregion

        #region Late Methods

        private readonly AbcLateMethodCollection _lateMethods = new AbcLateMethodCollection();

    	void AddLateMethod(AbcMethod method, AbcCoder coder)
        {
            _lateMethods.Add(method, coder);
        }

        #endregion

        #region Utils

        IType FindTypeDefOrRef(string fullname)
        {
            return AssemblyIndex.FindType(_assembly, fullname);
        }

        AbcInstance FindInstanceDefOrRef(string fullname)
        {
            var type = FindTypeDefOrRef(fullname);
            if (type == null) return null;
            return DefineAbcInstance(type);
        }

        public AbcInstance FindInstanceRef(AbcMultiname name)
        {
            return AssemblyIndex.FindInstance(_assembly, name);
        }

		public AbcInstance ImportType(string fullname)
		{
			return ImportType(fullname, false);
		}

        public AbcInstance ImportType(string fullname, bool safe)
        {
        	try
        	{
				return _abc.ImportType(_assembly, fullname);
        	}
        	catch (Exception)
        	{
				if (safe)
				{
					CompilerReport.Add(Warnings.UnableImportType, fullname);
					return null;
				}
        		throw;
        	}            
        }

		public AbcInstance ImportType(string fullname, ref AbcInstance field)
		{
			return ImportType(fullname, ref field, false);
		}

		public AbcInstance ImportType(string fullname, ref AbcInstance field, bool safe)
		{
			return field ?? (field = ImportType(fullname));
		}

    	void AddMethod(AbcMethod method)
        {
            _abc.AddMethod(method);
        }

        public AbcParameter CreateParam(AbcMultiname type, string name)
        {
            return _abc.DefineParam(type, name);
        }

        public AbcParameter CreateParam(AbcInstance type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return CreateParam(type.Name, name);
        }

        public AbcParameter CreateParam(IType type, string name)
        {
            var typeName = DefineMemberType(type);
            return CreateParam(typeName, name);
        }

        public AbcParameter CreateParam(AvmTypeCode type, string name)
        {
            return _abc.DefineParam(type, name);
        }

    	#endregion

        #region NativeAPI Extensions
        AbcCode _newAPI;
        #endregion
    }
}