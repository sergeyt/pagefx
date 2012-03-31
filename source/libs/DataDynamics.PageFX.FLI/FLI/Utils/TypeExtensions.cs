using System;
using System.Linq;
using DataDynamics.PageFX.CodeModel;
using DataDynamics.PageFX.FLI.ABC;

namespace DataDynamics.PageFX.FLI
{
    /// <summary>
    /// Contains various type utils.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Returns true if given type should not be compiled.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanExclude(this IType type)
        {
            if (type.IsModuleType())
                return true;

            if (type.IsSpecialName || type.IsRuntimeSpecialName)
                return true;

            if (type.IsArrayInitializer())
                return true;

            if (type.IsPrivateImplementationDetails())
            {
            	return type.Fields.All(field => field.Type.IsArrayInitializer());
            }

        	return false;
        }

        //class <PrivateImplementationDetails>{C05318BA-D3C5-45BA-8FEC-725F72EE7B81}
        public static bool IsModuleType(this IType type)
        {
            if (type == null) return false;
            if (!type.IsClass) return false;
            if (type.DeclaringType != null) return false;
            return type.FullName == "<Module>";
        }

        public static bool IsPrivateImplementationDetails(this IType type)
        {
            if (type == null) return false;
            if (!type.IsCompilerGenerated) return false;
            if (!type.IsClass) return false;
            if (type.DeclaringType != null) return false;
            string name = type.FullName;
            int n = name.Length;
            if (n == 0) return false;
            return name.StartsWith("<PrivateImplementationDetails>{") && name[n - 1] == '}';
        }

        /// <summary>
        /// Determines whether given type is array initializer struct.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArrayInitializer(this IType type)
        {
            if (type == null) return false;
            if (type.TypeKind != TypeKind.Struct) return false;
            if (type.Layout == null) return false;
            if (type.DeclaringType.IsPrivateImplementationDetails()) return true;
            return false;
        }

        /// <summary>
        /// Finds type in given assembly.
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static IType FindType(IAssembly asm, string fullname)
        {
            var t = asm.FindType(fullname);
            if (t == null)
                throw new InvalidOperationException(
                    String.Format("Unable to find type {0}.", fullname));
            return t;
        }

        /// <summary>
        /// Determines whether we should use native AVM Object type when given type is used for member type
        /// </summary>
        /// <param name="type">given type to inspect</param>
        /// <returns></returns>
        public static bool UseNativeObject(this IType type)
        {
            if (type == null) return false;

            if (type.TypeKind == TypeKind.Reference)
                return true;

            if (type == SystemTypes.Object)
                return true;

            if (AbcGenConfig.UseAvmString && type.IsStringInterface())
                return true;

            if (type.IsGenericArrayInterface())
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the given type has only one instance constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasSingleConstructor(this IType type)
        {
            if (type == null) return false;
            if (type.IsInterface) return false;
            int n = 0;
            foreach (var m in type.Methods.Where(m => !m.IsStatic && m.IsConstructor))
            {
            	if (n >= 1) return false;
            	++n;
            }
            return true;
        }

        public static bool IsNativeType(this IType type, string fullname)
        {
            if (type == null) return false;
            var instance = type.Tag as AbcInstance;
            if (instance == null) return false;
            if (!instance.IsNative) return false;
            return instance.FullName == fullname;
        }

        public static AbcMultiname ToMultiname(this object typeTag)
        {
            if (typeTag == null) return null;
            var instance = typeTag as AbcInstance;
            if (instance != null) return instance.Name;
            var vec = typeTag as IVectorType;
            if (vec != null) return vec.Name;
            return typeTag as AbcMultiname;
        }

        public static AbcMultiname GetMultiname(this IType type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type.Tag.ToMultiname();
        }

        public static bool IsStringInterface(this IType type)
        {
            return SystemTypes.String.Implements(type);
        }

        public static bool IsInternalType(this IType type)
        {
            if (String.IsNullOrEmpty(type.Namespace) && type.Name == "avm")
                return true;
            return false;
        }

        public static bool HasGlobalFunctions(this IType type)
        {
            return Attrs.Has(type, Attrs.GlobalFunctions);
        }

        #region name utils
        public static string GetPartialTypeName(IType type)
        {
            return type.NestedName;
        }

        public static string GetNamespaceForMembers(string ns, string name)
        {
            if (String.IsNullOrEmpty(ns))
                return name;
            return ns + ":" + name;
        }

        public static string GetNamespaceForMembers(IType type)
        {
            string name = GetPartialTypeName(type);
            string ns = NameUtil.GetTypeNamespace("", type);
            return GetNamespaceForMembers(ns, name);
        }

        public static string GetNamespaceForMembers(AbcInstance instance)
        {
            return GetNamespaceForMembers(instance.NamespaceString, instance.NameString);
        }
        #endregion

        #region ctor utils
        public static IMethod FindConstructor(this IType type, Func<IMethod, bool> ctorPredicate)
        {
            if (type == null) return null;
        	return type.Methods.Constructors.FirstOrDefault(m => !m.IsStatic && ctorPredicate(m));
        }

        public static IMethod FindConstructor(this IType type, int argCount)
        {
            return FindConstructor(type, ctor => ctor.Parameters.Count == argCount);
        }

        public static IMethod FindParameterlessConstructor(this IType type)
        {
            return FindConstructor(type, 0);
        }

        public static IMethod FindParameterlessConstructor(AbcInstance instance)
        {
            var type = instance.Type;
            return type == null ? null : FindParameterlessConstructor(type);
        }

        public static bool AllowNonParameterlessInitializer(IType type)
        {
            if (AbcGenConfig.IsInitializerParameterless)
                return false;
            //NOTE: ctor can be call as usual method on value types
            if (type.TypeKind == TypeKind.Struct)
                return false;
            return true;
        }

        public static IMethod FindInitializer(AbcInstance instance)
        {
            if (instance == null) return null;
            var type = instance.Type;
            if (type == null) return null;
            return FindInitializer(type);
        }

        public static IMethod FindInitializer(IType type)
        {
            if (type == null) return null;
            IMethod ctor = null;
            int n = 0;
            foreach (var m in type.Methods.Constructors)
            {
                if (m.IsStatic) continue;
                if (n >= 1) return null;
                if (m.Parameters.Count > 0 && !AllowNonParameterlessInitializer(type))
                    return null;
                ctor = m;
                ++n;
            }
            return ctor;
        }

        public static IMethod FindStaticCtor(IType type)
        {
            if (type == null) return null;
            if (type.IsArray)
                type = SystemTypes.Array;
            return type.Methods.StaticConstructor;
        }

        public static int GetCtorCount(IType type)
        {
        	return type.Methods.Constructors.Count(m => !m.IsStatic);
        }

    	public static bool CanUseEmptyCtor(IType type)
        {
            if (type == null) return false;
            var st = type.SystemType;
            if (st == null) return false;
            switch (st.Code)
            {
                case SystemTypeCode.Int8:
                case SystemTypeCode.Int16:
                case SystemTypeCode.Int32:
                case SystemTypeCode.Int64:
                case SystemTypeCode.UInt8:
                case SystemTypeCode.UInt16:
                case SystemTypeCode.UInt32:
                case SystemTypeCode.UInt64:
                case SystemTypeCode.Single:
                case SystemTypeCode.Double:
                case SystemTypeCode.Decimal:
                    return true;
            }
            return false;
        }
        #endregion

        #region numeric types
        public static bool IsInt64(this IType type)
        {
            return type == SystemTypes.Int64 || type == SystemTypes.UInt64;
        }

        public static bool IsDecimal(this IType type)
        {
            return type == SystemTypes.Decimal;
        }

        public static bool IsDecimalOrInt64(this IType type)
        {
            return IsDecimal(type) || IsInt64(type);
        }

        public static bool IsDecimalOrInt64(IType type1, IType type2)
        {
            return IsDecimalOrInt64(type1) || IsDecimalOrInt64(type2);
        }

        public static IType SelectDecimalOrInt64(IType type1, IType type2)
        {
            if (IsDecimal(type1))
                return type1;
            if (IsDecimal(type2))
                return type2;
            if (IsInt64(type1))
                return type1;
            if (IsInt64(type2))
                return type2;
            return null;
        }

        public static NumberType GetNumberType(this IType type)
        {
            var st = type.SystemType;
            if (st != null)
            {
                switch (st.Code)
                {
                    case SystemTypeCode.Int8:
                    case SystemTypeCode.Int16:
                    case SystemTypeCode.Int32:
                        return NumberType.Int32;

                    case SystemTypeCode.UInt8:
                    case SystemTypeCode.UInt16:
                    case SystemTypeCode.UInt32:
                        return NumberType.UInt32;

                    case SystemTypeCode.Int64:
                        return NumberType.Int64;

                    case SystemTypeCode.UInt64:
                        return NumberType.UInt64;
                }
            }
            return NumberType.Float;
        }
        #endregion

        public static bool IsBoxableOrInt64Based(this IType type)
        {
            return type.IsBoxableType() || IsInt64Based(type);
        }

        public static bool IsInt64Based(this IType type)
        {
            if (type.IsEnum)
                type = type.ValueType;
            return type == SystemTypes.Int64 || type == SystemTypes.UInt64;
        }

        public static bool HasCopy(IType type)
        {
            if (type == null) return false;
            if (type == SystemTypes.ValueType) return false;
            if (type == SystemTypes.Enum) return false;
            if (type.TypeKind == TypeKind.Struct) return true;
            return IsDecimalOrInt64(type);
        }

        public static bool MustInitValueTypeFields(IType type)
        {
            if (type == null) return false;
            if (type.IsBoxableType()) return false;
            if (type.IsEnum) return false;
            if (type.IsInterface) return false;
            if (type.IsArray) return false;
            var st = type.SystemType;
            if (st != null)
            {
                switch (st.Code)
                {
                    case SystemTypeCode.Array:
                    case SystemTypeCode.DateTime:
                    case SystemTypeCode.Delegate:
                        return false;

                    case SystemTypeCode.Int64:
                    case SystemTypeCode.UInt64:
                        return false;
                }
            }
            return true;
        }

        public static bool IsInitArray(IType type)
        {
            if (type == null) return false;
            switch (type.TypeKind)
            {
                case TypeKind.Primitive:
                case TypeKind.Struct:
                case TypeKind.Enum:
                    return true;
            }
            return IsInitRequired(type);
        }

        public static bool IsInitRequired(IType type)
        {
            if (type.TypeKind == TypeKind.Struct)
            {
                return !type.IsArrayInitializer();
            }
            if (type.IsEnum)
                type = type.ValueType;
            var st = type.SystemType;
            if (st != null)
            {
                switch (st.Code)
                {
                    case SystemTypeCode.Int64:
                    case SystemTypeCode.UInt64:
                    case SystemTypeCode.Decimal:
                    case SystemTypeCode.DateTime:
                        return true;
                }
            }
            return false;
        }

        public static bool IsInitRequiredField(IType type)
        {
            if (IsInitRequired(type)) return true;
            if (type == SystemTypes.Double) return true;
            if (type == SystemTypes.Single) return true;
            return false;
        }

        public static bool HasInitFields(IType type, bool isStatic)
        {
            if (!MustInitValueTypeFields(type)) return false;
        	return type.Fields.Any(f => !f.IsConstant && f.IsStatic == isStatic && IsInitRequiredField(f.Type));
        }

        public static IType GetElementType(this IType type)
        {
            var compoundType = type as ICompoundType;
            if (compoundType == null) throw new ArgumentException("type");
            return compoundType.ElementType;
        }

        public static bool IsFrom(IType type, string fullname)
        {
            if (type == null) return false;
            var bt = type.BaseType;
            while (bt != null)
            {
                if (bt.FullName == fullname)
                    return true;
                bt = bt.BaseType;
            }
            return false;
        }

        public static bool IsAvmObject(this IType type)
        {
            if (type == null) return false;
            var instance = type.Tag as AbcInstance;
            if (instance != null)
                return instance.IsObject;
            return false;
            //string name = type.FullName;
            //return name == "Avm.Object" || name == "Object";
        }

        public static bool IsFromAvmObject(this IType type)
        {
            if (type == null) return false;
            var bt = type.BaseType;
            while (bt != null)
            {
                if (bt.IsAvmObject())
                    return true;
                bt = bt.BaseType;
            }
            return false;
        }

        public static bool IsRootSprite(this IType type)
        {
            if (type == null) return false;

            if (type.CustomAttributes.Any(attr => attr.TypeName == Attrs.RootSprite))
            {
                return true;
            }

            return false;
        }

        public static bool IsValueType(this IType type)
        {
            if (type == null) return false;
            if (type.TypeKind == TypeKind.Struct) return true;
            if (type.IsEnum) return true;
            if (type.TypeKind == TypeKind.Primitive) return true;
            return false;
        }

        public static bool MustUseCastToMethod(IType type, bool asop)
        {
            if (type == null) return false;
            if (type.IsArray) return true;

            if (AbcGenConfig.UseCastToValueType && asop && type.IsValueType()) return true;

            if (AbcGenConfig.UseAvmString && type.IsStringInterface())
                return true;

            if (type.IsGenericArrayInterface())
                return true;

            if (type.IsNullableInstance())
                return true;

            
            return false;
        }

    	public static IField[] GetEnumFields(this IType type)
    	{
    		if (type == null)
    			throw new ArgumentNullException("type");
    		if (type.TypeKind != TypeKind.Enum)
    			throw new ArgumentException("type is not enum");
    		return type.Fields.Where(f => f.IsStatic).ToArray();
    	}

    	public static bool HasProtectedNamespace(this IType type)
    	{
    		switch (type.TypeKind)
    		{
    			case TypeKind.Interface:
    			case TypeKind.Enum:
    				return false;
    		}
    		return true;
    	}
    }
}