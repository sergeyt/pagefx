using System;
using System.Collections.Generic;
using System.Linq;
using DataDynamics.PageFX.Common.Extensions;
using DataDynamics.PageFX.Common.TypeSystem;
using DataDynamics.PageFX.FlashLand.Abc;
using DataDynamics.PageFX.FlashLand.Core.Tools;

namespace DataDynamics.PageFX.FlashLand.Core
{
	//TODO: lazy caching of types/instances

    internal sealed class AssemblyIndex
    {
	    private readonly IAssembly _assembly;
	    private readonly Dictionary<string, IType> _typeCache = new Dictionary<string, IType>();

        public static object ResolveRef(IAssembly assembly, string id)
        {
            string name = id.Replace(':', '.');
            var instance = FindInstance(assembly, name);
            if (instance != null)
            {
                if (instance.InSwc)
                    return instance.Abc;
            }
            return instance;
        }

        public static void Setup(IAssembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var data = assembly.CustomData();
            if (data.Index != null) return;

            data.Index = new AssemblyIndex(assembly);
        }

        public static IType FindType(IAssembly assembly, string name)
        {
            Setup(assembly);

            var index = assembly.CustomData().Index;
            if (index != null)
                return index.FindTypeCore(name);

            return null;
        }

        public static IType FindType(IAssembly assembly, AbcMultiname name)
        {
        	return name.GetFullNames().Select(fullName => FindType(assembly, fullName)).FirstOrDefault(type => type != null);
        }

    	public static AbcInstance FindInstance(IAssembly asm, string name)
        {
            Setup(asm);

            var index = asm.CustomData().Index;
            if (index != null)
                return index.FindInstanceCore(name);

            return null;
        }

        public static AbcInstance FindInstance(IAssembly asm, AbcMultiname name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.IsRuntime) 
                return null;
            if (name.NamespaceSet != null)
            {
            	return name.NamespaceSet
					.Select(ns => ns.NameString.MakeFullName(name.NameString))
					.Select(fullName => FindInstance(asm, fullName))
					.FirstOrDefault(instance => instance != null);
            }
        	return FindInstance(asm, name.FullName);
        }

        private AssemblyIndex(IAssembly assembly)
        {
			if (assembly == null)
				throw new ArgumentNullException("assembly");

	        _assembly = assembly;

            Build(assembly);
        }

	    private void Build(IAssembly root)
	    {
		    Link(root);

		    root.ProcessReferences(true, assembly => { Link(assembly); });
	    }

	    private void Link(IAssembly assembly)
		{
			var data = assembly.CustomData();
			data.Index = this;

			if ((data.Flags & InternalAssembyFlags.PassedLinker) == 0)
			{
				var linker = new Linker(assembly);
				linker.TypeLinked += OnTypeLinked;
				linker.Run();
			}
			else
			{
				data.Linker.TypeLinked += OnTypeLinked;
			}

			if (assembly.Loader == null)
			{
				RegisterTypes(assembly.Types);
			}
		}

	    private void OnTypeLinked(object sender, TypeEventArgs e)
	    {
		    RegisterType(e.Type);
	    }

	    private void RegisterTypes(IEnumerable<IType> types)
        {
            foreach (var type in types)
            {
                RegisterType(type);
            }
        }

	    private void RegisterType(IType type)
	    {
		    string name = type.FullName;
			if (!_typeCache.ContainsKey(name))
				_typeCache.Add(name, type);

		    var instance = type.Tag as AbcInstance;
		    if (instance != null)
		    {
			    string name2 = instance.FullName;
			    if (name2 != name)
					_typeCache.Add(name2, type);
		    }
	    }

	    private IType FindTypeCore(string name)
        {
            if (name == null) return null;

		    IType type;
		    if (_typeCache.TryGetValue(name, out type))
		    {
			    return type;
		    }

		    type = _assembly.GetReferences(false).Select(x => x.FindType(name)).FirstOrDefault(x => x != null);
			if (type != null)
			{
				return type;
			}

		    return null;
        }

        private AbcInstance FindInstanceCore(string name)
        {
            var type = FindTypeCore(name);
            if (type != null)
                return type.Tag as AbcInstance;
            return null;
        }
    }
}