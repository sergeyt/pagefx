using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using DataDynamics.PageFX.Common.Extensions;
using DataDynamics.PageFX.Common.TypeSystem;
using DataDynamics.PageFX.FLI.ABC;
using DataDynamics.PageFX.FLI.IL;
using DataDynamics.PageFX.FLI.SWF;

#region SWF Structure
//FileAttributes
//Metadata
//EnableDebugger2
//DebugID - found in swc components, may be generated by Flex
//ScriptLimits
//SetBackgroundColor
//ProductInfo
//-- Per Frame:
//  FrameLabel
//  {Assets}
//  {DoAbc2}
//  SymbolClass
//ShowFrame
#endregion

#region SWC (zip file) Structure
//library.swf - tags:
//  FileAttributes
//  Metadata
//  EnabledDebugger2
//  DebugID
//  ScriptLimits
//  SetBackgroundColor
//  ProductInfo
//  {Assets}
//  {DoAbc2}
//  SymbolClass - for assets (no zero symbol for root sprite)
//  ShowFrame
//catalog.xml
#endregion

namespace DataDynamics.PageFX.FLI
{
    partial class SwfCompiler : IDisposable
    {
        #region ctors
        public SwfCompiler(SwfCompilerOptions options)
        {
            _options = options ?? new SwfCompilerOptions();
        }
        #endregion

        #region Options
        readonly SwfCompilerOptions _options;

        public SwfCompilerOptions Options
        {
            get { return _options; }
        }

        public bool IsSwc
        {
            get { return _options.OutputFormat == OutputFormat.SWC; }
        }
        
        public string RootSprite
        {
            get { return _options.RootSprite; }
        }

        public int PlayerVersion
        {
            get { return _options.FlashVersion; }
        }

        public string OutputPath
        {
            get { return _options.OutputPath; }
        }

        public string OutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_options.OutputPath))
                    return "";
                return Path.GetDirectoryName(_options.OutputPath);
            }
        }

        public string Title
        {
            get { return _options.Title; }
        }

        public string Application
        {
            get { return _options.Application; }
        }

        public float Width
        {
            get { return _options.FrameSize.Width; }
        }

        public float Height
        {
            get { return _options.FrameSize.Height; }
        }

        public string HexBgColor
        {
            get
            {
	            Color color = _options.BackgroundColor;
	            return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
            }
        }

        public string RootNamespace
        {
            get { return _options.RootNamespace; }
        }
        #endregion

        #region Shared Members
        public static void Save(IAssembly assembly, string path, SwfCompilerOptions options)
        {
            if (options == null)
                options = new SwfCompilerOptions();
            options.OutputPath = path;
            using (var compiler = new SwfCompiler(options))
            {
                compiler.Build(assembly);
                compiler.Save(path);
            }
        }

        public static void Save(IAssembly assembly, Stream output, SwfCompilerOptions options)
        {
            if (options == null)
                options = new SwfCompilerOptions();
            using (var compiler = new SwfCompiler(options))
            {
                compiler.Build(assembly);
                compiler.Save(output);
            }
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
        ~SwfCompiler()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
        #endregion

        #region Entry Points
        internal SwfMovie _swf;

        public IAssembly ApplicationAssembly
        {
            get { return _assembly; }
        }
        IAssembly _assembly;

        bool GenerateHtmlWrapper = true;

        #region Build
        public void Build(IAssembly assembly)
        {
            _assembly = assembly;

            _swf = new SwfMovie
                       {
                           Name = "PageFX Application"
                       };

            InitPrerequisites();
            SetupHeader();
            SetFileAttributes();
            SetupMetadata();
            SetupDebugInfo();
            SetupScriptLimits();
            SetBackgroundColor();
            SetupProductInfo();

            GenearateFrames();
            
            if (IsSwc)
            {
                BuildCatalog();
                BuildSwc();
            }
            else
            {
                if (GenerateHtmlWrapper && !_options.NoHtmlWrapper)
                    HtmlTemplate.Deploy(this);
            }
        }
        #endregion

        #region Save
        void CheckSwf()
        {
            if (_swf == null)
                throw new InvalidOperationException("SWF movie is not generated yet");
        }

        public void Save(string path)
        {
            CheckSwf();

            if (IsSwc)
                SaveSwc(path);
            else
                _swf.Save(path);
        }

        public void Save(Stream output)
        {
            CheckSwf();

            if (IsSwc)
                SaveSwc(output);
            else
                _swf.Save(output);
        }
        #endregion
        #endregion

        #region InitPrerequisites
        void InitPrerequisites()
        {
            var tag = AssemblyTag.Instance(_assembly);
            tag.SWF = _swf;
            AssemblyIndex.Setup(_assembly);
            LinkRsls();
        }

        IEnumerable<IAssembly> GetRefs()
        {
            return _assembly.GetReferences(true);
        }

        void LinkRsls()
        {
            var list = _options.RslList;
            for (int i = 0; i < list.Count; ++i)
            {
                var rsl = list[i];
                var asm = GetRefs().FirstOrDefault(
                    r =>
                        {
                            string rpath = r.Location;
                            if (!Path.IsPathRooted(rpath))
                                rpath = Path.Combine(Environment.CurrentDirectory, rpath);
                            if (rpath.ComparePath(rsl.Library) == 0)
                                return true;
                            return false;
                        });

                if (asm == null)
                {
                    //RSL is not referneced by assembly
                    list.RemoveAt(i);
                    --i;
                }
                else
                {
                    var tag = AssemblyTag.Instance(asm);
                    tag.RSL = rsl;
                    if (tag.SWC == null)
                        throw Errors.RSL.SwcIsNotResolved.CreateException(rsl.LocalPath);

                    tag.SWC.RSL = rsl;
                    rsl.Swc = tag.SWC;
                }
            }
        }
        #endregion

        #region Header
        void SetupHeader()
        {
            _swf.Version = _options.FlashVersion;
            _swf.FrameSize = _options.FrameSize;
            _swf.FrameRate = _options.FrameRate;
            _swf.AllowCompression = _options.Compressed;
            _swf.AutoFrameCount = false;
        }
        #endregion

        #region File Attributes
        void SetFileAttributes()
        {
            //TODO: Customize
            _swf.SetDefaultFileAttributes();
        }
        #endregion

        #region File Metadata
        void SetupMetadata()
        {
            string rdf = typeof(SwfCompiler).GetTextResource("pfc.rdf");
            _swf.Tags.Add(new SwfTagMetadata(rdf));
        }
        #endregion

        #region DebugInfo
        void SetupDebugInfo()
        {
            if (_options.Debug)
            {
                _swf.EnableDebugger(6517, _options.DebugPassword);
                _swf.Tags.Add(new SwfTagDebugID("7ae6b0e5-298b-42a8-01d9-a2a555be7ef8"));
            }
        }
        #endregion

        #region ScriptLimits
        void SetupScriptLimits()
        {
            _swf.SetDefaultScriptLimits();
        }
        #endregion

        #region BackgroundColor
        void SetBackgroundColor()
        {
            _swf.SetBackgroundColor(_options.BackgroundColor);
        }
        #endregion

        #region ProductInfo
        private void SetupProductInfo()
        {
        	_swf.Tags.Add(new SwfTagProductInfo
        	              	{
        	              		ProductID = 1,
        	              		Edition = 0,
        	              		MajorVersion = 1,
        	              		MinorVersion = 0,
        	              		BuildNumber = 0,
        	              		BuildDate = (ulong)DateTime.Now.Ticks
        	              	});
        }
        #endregion

        #region Frames
        internal readonly List<AbcFile> AbcFrames = new List<AbcFile>();

		/// <summary>
		/// Frame with Flex SystemManager.
		/// </summary>
        internal AbcFile FrameMX
        {
            get { return _frameMX; }
            set
            {
                if (_frameMX != null)
                    throw Errors.Internal.CreateException();
                _frameMX = value;
                AbcFrames.Add(value);
                AssemblyTag.Instance(_assembly).AddAbc(value);
            }
        }
        AbcFile _frameMX;

		/// <summary>
		/// Application frame.
		/// </summary>
        internal AbcFile FrameApp
        {
            get { return _frameApp; }
            set
            {
                if (value != _frameApp)
                {
                    _frameApp = value;
                    value.PrevFrame = FrameMX;
                    AbcFrames.Add(value);
                }
            }
        }
        AbcFile _frameApp;

        void GenearateFrames()
        {
            GenerateMxSystemManagerFrame();
            GenerateApplicationFrame();
            _lateMethods.Finish();
        }
        #endregion

        #region Application Frame
        void GenerateApplicationFrame()
        {
            _swf.FrameCount++;

            var g = new AbcGenerator {sfc = this};
            var abc = g.Generate(_assembly);

            if (IsSwc)
            {
                var symTable = new SwfTagSymbolClass();

                CreateScripts(abc, symTable);

                if (symTable.Symbols.Count > 0)
                    _swf.Tags.Add(symTable);
            }
            else
            {
                var rootName = IsFlexApplication ? _typeFlexApp.FullName : g.RootSprite.FullName;

                //label should be the same as root name
                _swf.SetFrameLabel(rootName);

                if (g.IsNUnit)
                    GenerateHtmlWrapper = false;

                var symTable = new SwfTagSymbolClass();
                //see http://bugs.adobe.com/jira/browse/ASC-3235
                AddAbcTag(abc);
                ImportLateAssets();
                FlushAssets(symTable);

                //NOTE: In MX application root sprite is autogenerated subclass of mx.managers.SystemManager.
                if (!IsFlexApplication)
                {
                    //define symbol for root sprite
                    symTable.AddSymbol(0, rootName);
                }

                if (symTable.Symbols.Count > 0)
                    _swf.Tags.Add(symTable);
            }

            _swf.ShowFrame();
        }

        void AddAbcTag(AbcFile abc)
        {
            string frameName = abc.Name;
            if (string.IsNullOrEmpty(frameName))
                frameName = "frame" + _swf.FrameCount;
            _swf.Tags.Add(new SwfTagDoAbc2(frameName, 1, abc));
        }
        #endregion

        #region mx.managers.SystemManager Frame
        // Any dependencies of SystemManager have to load in frame 1,
        // before the preloader, or anything else, can be displayed.

        //We need to autogenerate subclass of mx.managers.SystemManager
        //and override the following methods:
        //create(... params):Object
        //info():Object - default is empty object (i.e {}).

        public IType TypeFlexApp
        {
            get
            {
                if (IsSwc) return null;
                if (_typeFlexApp != null)
                    return _typeFlexApp;
                if (_searchFlexAppType)
                {
                    //TODO: Resolve the situation: assembly can have more than one subclasses of mx.core.Application
                    _searchFlexAppType = false;
                    var apps = _assembly.Types.Where(type => InternalTypeExtensions.IsFrom(type, "mx.core.Application")).ToList();
                    int n = apps.Count;
                    if (n > 0)
                    {
                        if (n == 1)
                        {
                            _typeFlexApp = apps[0];
                        }
                        else
                        {
							_typeFlexApp = apps.FirstOrDefault(InternalTypeExtensions.IsRootSprite);
                            if (_typeFlexApp == null)
                                throw new AmbiguousMatchException("Unable to find MX application class");
                        }
                    }
                }
                return _typeFlexApp;
            }
        }
        private IType _typeFlexApp;
		private bool _searchFlexAppType = true;

        /// <summary>
        /// Auto detects whether translated assembly is flex application.
        /// </summary>
        public bool IsFlexApplication
        {
            get 
            {
                if (IsSwc) return false;
                return TypeFlexApp != null;
            }
        }

        public string[] Locales
        {
            get { return _options.Locales; }
        }

        private void GenerateMxSystemManagerFrame()
        {
            if (IsSwc) return;

        	if (BuildMxFrame())
            {
                Debug.Assert(FrameMX != null);
                _swf.FrameCount++;

                _swf.SetFrameLabel("System Manager");

                var symTable = new SwfTagSymbolClass();
                //FlushAssets(symTable);
                AddAbcTag(FrameMX);
                ImportLateAssets();
                FlushAssets(symTable);

                symTable.AddSymbol(0, _mxSystemManager.FullName);
                _swf.Tags.Add(symTable);
                _swf.ShowFrame();
            }
        }
        #endregion

        #region Late Methods
        readonly AbcLateMethodCollection _lateMethods = new AbcLateMethodCollection();

        void AddLateMethod(AbcMethod method, AbcCoder coder)
        {
            _lateMethods.Add(method, coder);
        }
        #endregion

        #region FinishApplication
        public void FinishApplication()
        {
            if (IsSwc) return;
            ImportMixins();
            //ImportLateAssets();
        }
        #endregion
    }
}