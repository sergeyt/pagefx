using System;
using System.Linq;
using DataDynamics.PageFX.Common.Extensions;
using DataDynamics.PageFX.Common.TypeSystem;
using DataDynamics.PageFX.Common.Utilities;
using DataDynamics.PageFX.FlashLand.Abc;
using DataDynamics.PageFX.FlashLand.Core.CodeProvider;
using DataDynamics.PageFX.FlashLand.Core.Tools;

namespace DataDynamics.PageFX.FlashLand.Core.CodeGeneration
{
	//Contains generation of AbcMethod
    //Key method (entry point): DefineMethod
    internal partial class AbcGenerator
    {
        #region Signature

        private AbcMultiname DefineQName(IMethod method)
        {
			string name = method.GetSigName(Runtime.Avm);
            return Abc.DefineQName(method, name);
        }

		//fixes due to flex compiler bugs
        private static string GetFixedName(IMethod method)
        {
            //NOTE: flex compiler lookups inside instance to resolve type refs
            var declType = method.DeclaringType;
            if (declType.Is(SystemTypeCode.DateTime))
            {
                //NOTE: Date property can hide global Date type therefore we must rename it.
                if (method.IsAccessor()
                    && method.Association.Name == "Date")
                    return "DATE";
            }
            else if (declType.Is(SystemTypeCode.Type))
            {
                //NOTE: Namespace property can hide global Namespace type therefore we must rename it.
                if (method.IsAccessor()
                    && method.Association.Name == "Namespace")
                    return "NAMESPACE";
            }
            return null;
        }

        private AbcMultiname GetMethodName(IMethod method, bool isOverride)
        {
            if (isOverride && method.BaseMethod != null)
            {
                var mn = GetDefinedMethodName(method.BaseMethod);
                if (mn != null) return mn;
            }

            var impls = method.Implements;
            if (impls != null && impls.Count == 1)
            {
	            return GetDefinedMethodName(impls[0]);
            }

            string name = GetFixedName(method) ?? method.GetSigName(Runtime.Avm);
        	return Abc.DefineQName(method, name);
        }

        private AbcMultiname GetDefinedMethodName(IMethod method)
        {
            var tag = DefineMethod(method);

            var m = tag as AbcMethod;
            if (m != null)
                return m.TraitName;

            var name = tag as AbcMultiname;
            if (name == null)
                throw new InvalidOperationException();

            return name;
        }
        
	    internal Sig SigOf(IMethod method)
		{
			bool isOverride = method.IsOverride();
			var name = GetMethodName(method, isOverride);

		    var sig = new Sig(name, method, method)
			    {
				    Kind = method.ResolveTraitKind(),
				    Source = method
			    };

			if (method.IsStaticCall())
			{
				sig.IsStatic = true;
				sig.IsVirtual = false;
			}
			else
			{
				sig.IsVirtual = method.IsVirtual;

				// exception actually inherited from avm Error
				if (method.DeclaringType.Is(SystemTypeCode.Exception))
				{
					if (method.IsObjectOverrideMethod())
						isOverride = false;
				}

				sig.IsOverride = isOverride;
			}

			return sig;
		}

        #endregion

        #region ImportMethod
        private object ImportMethod(AbcMethod method)
        {
            if (method.IsNative)
                return method;

            var abc = method.Abc;
            if (abc.IsCore)
                return method;

            if (method.ImportedMethod != null)
                return method.ImportedMethod;

            var instance = method.Instance;
            if (instance != null)
            {
                var i2 = Abc.ImportInstance(instance, ref method);
                if (i2 == null)
                    throw new InvalidOperationException();
            }
            else
            {
                if (!(method.Owner is AbcScript))
                    throw new InvalidOperationException();

                Abc.Import(abc);

                //NOTE: ABC can be linked externally
                if (method.ImportedMethod != null)
                    return method.ImportedMethod;
            }

            return method;
        }

        private object ImportMethod(IMethod method)
        {
            var tag = method.Data;
            if (tag == null) return null;
            var m = tag as AbcMethod;
            if (m != null)
            {
                tag = ImportMethod(m);
                DefineOverrideMethods(method, tag as AbcMethod);
                return tag;
            }
            throw new InvalidOperationException();
        }
        #endregion

        #region DefineMethod
        private object IsDefined(IMethod method)
        {
            if (Abc.IsDefined(method))
            {
                return method.Data;
            }

            var m = method.AbcMethod();
            if (m != null)
            {
                if (m.ImportedMethod != null)
                    return m.ImportedMethod;

                //external linking
                if (m.IsImported)
                    return m;
            }

            return null;
        }

        /// <summary>
        /// Defines given method.
        /// </summary>
        /// <param name="method">method to define.</param>
        /// <returns></returns>
        public object DefineMethod(IMethod method)
        {
            if (method == null) return null;

			if (method.IsGeneric)
                throw new InvalidOperationException();

            var tag = IsDefined(method);
            if (tag != null) return tag;

			var type = method.DeclaringType;
            DefineType(type);

            tag = IsDefined(method);
            if (tag != null) return tag;

            CheckApiCompatibility(method);

            tag = ImportMethod(method);
            if (tag != null) return tag;

            tag = CallResolver.Resolve(method);
            if (tag != null) return tag;

            DefineBaseMethods(method);

            //Define method signature types.
            DefineType(method.Type);
            foreach (var p in method.Parameters)
                DefineType(p.Type);

            tag = IsDefined(method);
            if (tag != null) return tag;

#if DEBUG
            DebugService.LogSeparator();
            DebugService.LogInfo("ABC DefineMethod started for method: {0}", method);
#endif

            var abcMethod = DefineMethodCore(method);

#if DEBUG
            DebugService.LogInfo("ABC DefineMethod succeded for method: {0}", method);
#endif
            return abcMethod;
        }
        #endregion

        #region DefineMethodCore
        /// <summary>
        /// Defines ABC method from managed method with normal body.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private AbcMethod DefineMethodCore(IMethod method)
        {
	        if (method.Data is AbcMethod)
			{
				throw new InvalidOperationException();
			}

            var declType = method.DeclaringType;
            var instance = declType.AbcInstance();
            if (instance == null)
                throw new InvalidOperationException();

			bool isMxAppCtor = false;
			if (AbcGenConfig.FlexAppCtorAsHandler)
				isMxAppCtor = IsFlexAppCtor(method);

			//NOTE:
			//1. ctor can be used as class or instance initializer
			//2. Static constructor will be compiled as normal method
			//3. Only default parameterless ctor will be compiled as initializer,
			//   all other ctors will be compiled as normal methods

			var isInitilizer = !GlobalSettings.ReflectionSupport && !isMxAppCtor && instance.Initializer == null && method.IsInstanceInitializer();
	        Sig sig;
			if (isInitilizer)
			{
				// initilizer has no name and return type
				sig = new Sig(null, null, method)
					{
						IsInitilizer = true,
						Source = method
					};
			}
			else
			{
				sig = SigOf(method);
				sig.IsAbstract = method.IsAbstract || method.Body == null; // without body
			}

			if (isMxAppCtor)
			{
				//HACK: Define mx.core.FlexEvent argument for MX app ctor
				var typeFlexEvent = FlexAppBuilder.FlexEventType();
				sig.Args = new object[] {typeFlexEvent.Name, "e"};
			}

	        return instance.DefineMethod(
		        sig, null,
		        abcMethod => CompleteMethod(instance, method, abcMethod));
        }

		private void CompleteMethod(AbcInstance instance, IMethod method, AbcMethod abcMethod)
		{
			DefineMethodBody(abcMethod);
			DefineImplementedMethods(method, instance, abcMethod);
			DefineOverrideMethods(method, abcMethod);
			ImplementProtoMethods(method, abcMethod);
		}

	    #endregion

        private void ImplementProtoMethods(IMethod method, AbcMethod abcMethod)
        {
            StringPrototypes.Implement(method);
            ObjectPrototypes.Implement(method, abcMethod);
        }

        #region DefineImplementedMethods
		/// <summary>
		/// Compiles <see cref="IMethod.Implements"/> methods.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="instance"></param>
		/// <param name="abcMethod"></param>
        private void DefineImplementedMethods(IMethod method, AbcInstance instance, AbcMethod abcMethod)
        {
            var impls = method.Implements;
            if (impls == null) return;

            int n = impls.Count;
            if (n <= 0) return;

            //NOTE: To avoid conflict with name of explicit implementation method has the same name as iface method
            if (method.IsExplicitImplementation)
            {
                DefineMethod(impls[0]);
                return;
            }

            if (n == 1 && !abcMethod.IsOverride)
            {
                DefineMethod(impls[0]);
                return;
            }

	        foreach (var ifaceMethod in impls)
	        {
		        var ifaceAbcMethod = DefineAbcMethod(ifaceMethod);
				DefineExplicitImplementation(instance, abcMethod, ifaceMethod, ifaceAbcMethod);
	        }
        }
        #endregion

        #region DefineReturnType
        public AbcMultiname DefineReturnType(AbcMethod abcMethod, IMethod method)
        {
            if (method.IsConstructor && method.AsStaticCall())
                return DefineMemberType(method.DeclaringType);

            var bm = GetBaseMethod(abcMethod, method);
            if (bm != null)
                return bm.ReturnType;

            return DefineReturnType(method.Type);
        }

        public AbcMultiname DefineReturnType(IType type)
        {
            if (type == null)
                return Abc.BuiltinTypes.Void;
            var name = DefineMemberType(type);
            if (name == null)
                throw new InvalidOperationException("Unable to define return type for method");
            return name;
        }
        #endregion

        #region DefineParameters, CopyParameters
        /// <summary>
        /// Defines parameters for given <see cref="AbcMethod"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source">source method of given <see cref="AbcMethod"/></param>
        public void DefineParameters(AbcMethod target, IMethod source)
        {
            if (source == EntryPoint)
            {
                if (AbcGenConfig.ParameterlessEntryPoint)
                    return;
            }

            if (source.HasPseudoThis())
            {
                var typeName = DefineMemberType(source.DeclaringType);
	            target.Parameters.Add(CreateParam(typeName, "this"));
            }

            var abm = GetBaseMethod(target, source);
            if (abm != null)
            {
                CopyParams(target, abm);
            }
            else
            {
                int n = source.Parameters.Count;
                for (int i = 0; i < n; ++i)
                {
#if DEBUG
                    DebugService.DoCancel();
#endif
                    var p = source.Parameters[i];
                    var ap = CreateParam(p.Type, p.Name);
	                target.Parameters.Add(ap);
                }
            }
        }

        private AbcMethod GetBaseMethod(AbcMethod abcMethod, IMethod method)
        {
            var impls = method.Implements;
            if (impls != null && impls.Count == 1)
                return DefineAbcMethod(impls[0]);

            var baseMethod = method.BaseMethod;
            if (abcMethod.IsOverride && baseMethod != null)
                return baseMethod.AbcMethod();

            return null;
        }

        internal void CopyParams(AbcMethod to, AbcMethod from)
        {
            int n = from.Parameters.Count;
            for (int i = 0; i < n; ++i)
            {
#if DEBUG
                DebugService.DoCancel();
#endif
                var bp = from.Parameters[i];
                var ap = new AbcParameter
                             {
                                 Type = Abc.ImportConst(bp.Type),
                                 Name = Abc.ImportConst(bp.Name)
                             };
                if (bp.IsOptional)
                {
                    ap.IsOptional = true;
                    ap.Value = Abc.ImportValue(bp.Value);
                    to.HasOptionalParams = true;
                }
	            to.Parameters.Add(ap);
            }
            if (from.NeedRest)
                to.NeedRest = true;
        }
        #endregion

        #region DefineBaseMethods
        private void DefineBaseMethods(IMethod method)
        {
            //            var declType = method.DeclaringType;
            //            if (declType.IsInterface) return;

            //            var baseType = declType.BaseType;
            //            while (baseType != null)
            //            {
            //#if DEBUG
            //                DebugService.DoCancel();
            //#endif
            //                var m = Method.FindMethod(baseType, method, false);
            //                DefineMethod(m);
            //                baseType = baseType.BaseType;
            //            }

            var bm = method.BaseMethod;
            while (bm != null)
            {
#if DEBUG
                DebugService.DoCancel();
#endif
                DefineMethod(bm);
                bm = bm.BaseMethod;
            }

            var impls = method.Implements;
            if (impls != null)
            {
                foreach (var impl in impls)
                    DefineMethod(impl);
            }
        }
        #endregion

        #region DefineOverrideMethods
        private void DefineOverrideMethods(IMethod method, AbcMethod abcMethod)
        {
	        if (!method.IsAbstract && !method.IsVirtual) return;

#if DEBUG
	        DebugService.DoCancel();
#endif

	        var type = method.DeclaringType;
	        var instance = type.AbcInstance();
	        if (instance == null)
		        throw new InvalidOperationException();

	        if (instance.IsInterface)
	        {
				//TODO: fix this problem
		        //warning: do not use for each since instance.Implementations is modifiable.
		        for (int i = 0; i < instance.Implementations.Count; ++i)
		        {
#if DEBUG
			        DebugService.DoCancel();
#endif
			        var impl = instance.Implementations[i];
			        DefineImplementation(impl, impl.Type, method, abcMethod);
		        }
	        }
	        else
	        {
		        DefineSubclassOverrideMethods(instance, method);
	        }
        }
        #endregion

        #region DefineSubclassOverrideMethods
        //Defines override methods in subclasses
        private void DefineSubclassOverrideMethods(AbcInstance instance, IMethod method)
        {
            var type = instance.Type;
            if (type.Is(SystemTypeCode.Enum)) return;

			//warning: do not use foreach since instance.Subclasses is modifiable collection.
            for (int i = 0; i < instance.Subclasses.Count; ++i)
            {
#if DEBUG
                DebugService.DoCancel();
#endif
                var subclass = instance.Subclasses[i];
                DefineOverrideMethod(subclass.Type, method);

#if DEBUG
                DebugService.DoCancel();
#endif
                DefineSubclassOverrideMethods(subclass, method);
            }
        }
        #endregion

        #region DefineOverrideMethod

	    private void DefineOverrideMethod(IType implType, IMethod method)
        {
            if (implType == null) return;
            var m = implType.FindOverrideMethod(method);
            if (m != null)
                DefineMethod(m);
        }

        #endregion

        #region DefineImplementation
        private void DefineImplementation(AbcInstance implInstance, IType implType, IMethod ifaceMethod, AbcMethod ifaceAbcMethod)
        {
			if (implInstance == null)
				throw new ArgumentNullException("implInstance");
			if (implInstance.IsInterface)
				return;

			if (implType == null)
				throw new ArgumentNullException("implType");
	        if (implType.IsInterface)
                return;

            var impl = implType.FindImplementation(ifaceMethod, true, false);

            if (impl == null)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to find implementation for method {0} in type {1}",
                                  ifaceMethod.FullName, implType.FullName));
            }

	        impl = impl.ResolveGenericInstance(implType, ifaceMethod);
            
            var abcImpl = DefineMethod(impl) as AbcMethod;

			// determine whether we should create explicit impl
			if (abcImpl == null || implInstance.IsForeign
				|| ReferenceEquals(impl.DeclaringType, implType)
				|| impl.IsExplicitImplementation
				|| impl.Implements.Any(x => x == ifaceMethod))
				return;

			// do not create explicit impl if interface is implemented in base type
	        var iface = ifaceMethod.DeclaringType;
	        var baseIfaces = implType.BaseTypes().SelectMany(x => x.Interfaces.SelectMany(i => i.Interfaces.Append(i)));
			if (baseIfaces.Any(x => ReferenceEquals(x, iface)))
				return;
			
			if (Equals(abcImpl.TraitName, ifaceAbcMethod.TraitName)
				|| (abcImpl.TraitName != null && ifaceAbcMethod.TraitName != null
				&& abcImpl.TraitName.IsGlobalName(ifaceAbcMethod.TraitName.NameString)
				&& HasFlashIfaceName(ifaceAbcMethod)))
				return;

	        DefineExplicitImplementation(implInstance, abcImpl, ifaceMethod, ifaceAbcMethod);
        }

		private static bool HasFlashIfaceName(AbcMethod method)
		{
			var mn = method.TraitName;
			if (mn == null) return false;
			if (!mn.IsQName) return false;
			if (mn.Namespace.Kind != AbcConstKind.PublicNamespace) return false;

			var instance = method.Instance;
			if (instance == null) return false;
			if (!instance.Name.IsQName) return false;

			var ns = instance.Name.Namespace.NameString;
			var name = instance.Name.NameString;
			ns = string.IsNullOrEmpty(ns) ? name : ns + ":" + name;

			return mn.Namespace.NameString == ns;
		}

        #endregion

        #region DefineExplicitImplementation

	    private static void DefineExplicitImplementation(AbcInstance instance, AbcMethod abcMethod,
	                                                     IMethod ifaceMethod, AbcMethod ifaceAbcMethod)
	    {
		    instance.DefineMethod(
			    Sig.@from(ifaceAbcMethod),
			    code =>
				    {
					    code.LoadThis();
					    code.LoadArguments(ifaceMethod);
					    code.Call(abcMethod);
					    if (ifaceAbcMethod.IsVoid) code.ReturnVoid();
					    else code.ReturnValue();
				    },
			    m =>
				    {
					    var isOverride =
						    instance.BaseInstances()
						            .FirstOrDefault(x => x.Traits.Contains(ifaceAbcMethod.TraitName, ifaceAbcMethod.Trait.Kind)) != null;

					    m.Trait.IsOverride = isOverride;

					    OverrideExplicitImplsInSubclasses(instance, ifaceAbcMethod);
				    });
	    }

		private static void OverrideExplicitImplsInSubclasses(AbcInstance instance, AbcMethod ifaceMethod)
		{
			var name = ifaceMethod.TraitName;
			foreach (var c in instance.Subclasses)
			{
				var t = c.Traits.Find(name, ifaceMethod.Trait.Kind);
				if (t != null)
				{
					t.IsOverride = true;
				}
			}
		}

	    #endregion

        #region DefineMethodBody

        private void DefineMethodBody(AbcMethod abcMethod)
        {
            if (abcMethod == null)
                throw new ArgumentNullException();

            var method = abcMethod.SourceMethod;
            if (method == null) return;
            if (method.Body == null) return;

#if DEBUG
            DebugService.DoCancel();
#endif

            BuildBodyCore(abcMethod, method);
        }

	    private void BuildBodyCore(AbcMethod target, IMethod source)
        {
			var targetBody = target.Body;
			if (targetBody == null)
			{
				targetBody = new AbcMethodBody(target);
				Abc.MethodBodies.Add(targetBody);
			}

#if DEBUG
            DebugService.DoCancel();
#endif
            var sourceBody = source.Body;

            var codeProvider = new CodeProviderImpl(this, target);

            var translator = sourceBody.CreateTranslator();
            if (translator == null)
                throw new InvalidOperationException("No IL translator");
            var il = translator.Translate(source, sourceBody, codeProvider);

#if DEBUG
            DebugService.DoCancel();
#endif

            targetBody.IL.Add(il);
            targetBody.Finish(Abc);
        }
        #endregion

        #region DefineAbcMethod
        public AbcMethod DefineAbcMethod(IMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            var m = DefineMethod(method) as AbcMethod;
            if (m == null)
                throw new InvalidOperationException(string.Format("Unable to define method: {0}", method));
            return m;
        }

        public AbcMethod DefineAbcMethod(IType type, Func<IMethod,bool> p)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (p == null)
                throw new ArgumentNullException("p");
        	var m = type.Methods.FirstOrDefault(x => p(x));
            if (m == null)
                throw new InvalidOperationException("Unable to find method by given predicate");
            return DefineAbcMethod(m);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, int argc)
        {
            var source = type.Methods.Find(name, argc);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, Func<IType,bool> arg1)
        {
            var source = type.Methods.Find(name, arg1);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, Func<IType,bool> arg1, Func<IType,bool> arg2)
        {
            var source = type.Methods.Find(name, arg1, arg2);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, Func<IType,bool> arg1, Func<IType,bool> arg2, Func<IType,bool> arg3)
        {
            var source = type.Methods.Find(name, arg1, arg2, arg3);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, IType arg1)
        {
            var source = type.Methods.Find(name, arg1);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, IType arg1, IType arg2)
        {
            var source = type.Methods.Find(name, arg1, arg2);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name, IType arg1, IType arg2, IType arg3)
        {
            var source = type.Methods.Find(name, arg1, arg2, arg3);
            return DefineAbcMethod(source);
        }

        public AbcMethod DefineAbcMethod(IType type, string name)
        {
            return DefineAbcMethod(type, name, 0);
        }

        public LazyValue<AbcMethod> LazyMethod(IType type, Func<IMethod,bool> p)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, p));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, int argc)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, argc));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, Func<IType, bool> arg1)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, arg1));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, Func<IType, bool> arg1, Func<IType, bool> arg2)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, arg1, arg2));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, Func<IType, bool> arg1, Func<IType, bool> arg2, Func<IType, bool> arg3)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, arg1, arg2, arg3));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, IType arg1)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, arg1));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, IType arg1, IType arg2)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, arg1, arg2));
        }

		public LazyValue<AbcMethod> LazyMethod(IType type, string name, IType arg1, IType arg2, IType arg3)
        {
            return new LazyValue<AbcMethod>(() => DefineAbcMethod(type, name, arg1, arg2, arg3));
        }
        #endregion

	    private bool IsFlexAppCtor(IMethod method)
        {
            if (method == null) return false;
            if (method.IsStatic) return false;
            if (!IsSwf) return false;
            if (!method.IsConstructor) return false;
            if (method.Parameters.Count != 0) return false;
            return ReferenceEquals(method.DeclaringType, SwfCompiler.TypeFlexApp);
        }
    }
}