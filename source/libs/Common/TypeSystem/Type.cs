using System;
using System.Collections.Generic;
using System.Text;
using DataDynamics.PageFX.Common.CodeModel;
using DataDynamics.PageFX.Common.Syntax;

namespace DataDynamics.PageFX.Common.TypeSystem
{
    /// <summary>
    /// Represents base class for user-defined types.
    /// </summary>
    public class TypeImpl : TypeMember, IType
    {
	    public TypeImpl()
        {
            _members = new TypeMemberCollection(this);
        }

        public TypeImpl(TypeKind kind) : this()
        {
            TypeKind = kind;
        }

	    #region IType Members
        /// <summary>
        /// Gets the module in which the current type is defined. 
        /// </summary>
        public override IModule Module { get; set; }

        /// <summary>
        /// Gets the kind of this member.
        /// </summary>
        public override MemberType MemberType
        {
            get { return MemberType.Type; }
        }

	    /// <summary>
	    /// Gets kind of the type.
	    /// </summary>
	    public TypeKind TypeKind { get; set; }

	    /// <summary>
        /// Gets or sets flag specifing that this type is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return GetModifier(Modifiers.Abstract); }
            set { SetModifier(value, Modifiers.Abstract); }
        }

        /// <summary>
        /// Gets or sets flag specifing that this type is sealed.
        /// </summary>
        public bool IsSealed
        {
            get { return GetModifier(Modifiers.Sealed); }
            set { SetModifier(value, Modifiers.Sealed); }
        }

        /// <summary>
        /// Returns true if CLR must initialize the class before first static field access.
        /// </summary>
        public bool IsBeforeFieldInit
        {
            get { return GetModifier(Modifiers.BeforeFieldInit); }
            set { SetModifier(value, Modifiers.BeforeFieldInit); }
        }

        /// <summary>
        /// Gets or sets the flag indicating wheher the type is generated by compiler.
        /// </summary>
        public bool IsCompilerGenerated
        {
            get { return GetModifier(Modifiers.CompilerGenerated); }
            set { SetModifier(value, Modifiers.CompilerGenerated); }
        }

        /// <summary>
        /// Determines whether this type is interface.
        /// </summary>
        public bool IsInterface
        {
            get { return TypeKind == TypeKind.Interface; }
        }

        /// <summary>
        /// Determines whether this type is class.
        /// </summary>
        public bool IsClass 
        {
            get { return TypeKind == TypeKind.Class; }
        }

        /// <summary>
        /// Determines whether this type is array.
        /// </summary>
        public bool IsArray
        {
            get
            {
                //NOTE: Only compund type can be array
                return false;
            }
        }

        /// <summary>
        /// Determines whether this type is enum type.
        /// </summary>
        public bool IsEnum
        {
            get { return TypeKind == TypeKind.Enum; }
        }

        public bool HasIEnumerableInstance { get; set; }

        /// <summary>
        /// Gets or sets type namespace
        /// </summary>
        public string Namespace
        {
            get
            {
                if (DeclaringType != null)
                    return DeclaringType.Namespace;
                return _namespace;
            }
            set { _namespace = value; }
        }
        private string _namespace;

        /// <summary>
        /// Gets the full name of this type.
        /// </summary>
        public override string FullName
        {
            get { return _fullName ?? (_fullName = FullNameBase + FullNameSuffix); }
        }
        private string _fullName;

        protected virtual string FullNameBase
        {
            get { return GetName(this, true); }
        }

        protected virtual string FullNameSuffix
        {
            get { return ""; }
        }

        public override string DisplayName
        {
            get
            {
                string k = CSharpKeyword;
                if (!string.IsNullOrEmpty(k))
                    return k;
                return FullName;
            }
        }

        /// <summary>
        /// Gets a <see cref="IMethod"/> that represents the declaring method, if the current <see cref="TypeImpl"/> represents a type parameter of a generic method.
        /// </summary>
        public IMethod DeclaringMethod { get; set; }

        /// <summary>
        /// Gets or sets base type.
        /// </summary>
        public IType BaseType { get; set; }

        /// <summary>
        /// Gets list of interfaces implemented by this type.
        /// </summary>
        public ITypeCollection Interfaces
        {
            get { return _interfaces ?? (_interfaces = new SimpleTypeCollection()); }
			set { _interfaces = value; }
        }
        private ITypeCollection _interfaces;

        public ITypeMemberCollection Members
        {
            get { return _members; }
        }
        private readonly TypeMemberCollection _members;

        public IFieldCollection Fields
        {
            get { return _members.Fields; }
        }

        public IMethodCollection Methods
        {
            get { return _members.Methods; }
        }

        public IPropertyCollection Properties
        {
            get { return _members.Properties; }
        }

        public IEventCollection Events
        {
            get { return _members.Events; }
        }

        public IType ValueType
        {
            get
            {
                if (TypeKind == TypeKind.Enum)
                {
                    if (_valueType == null)
                    {
                        foreach (var field in Fields)
                        {
                            if (field.IsSpecialName)
                            {
                                _valueType = field.Type;
                                break;
                            }
                        }
                    }
                }
                return _valueType;
            }
        }
        private IType _valueType;

        public ClassLayout Layout { get; set; }

        /// <summary>
        /// Gets or sets members defined with syntax of some language
        /// </summary>
        public string CustomMembers { get; set; }

        /// <summary>
        /// Gets or sets type source code.
        /// </summary>
        public string SourceCode { get; set; }
        #endregion

	    /// <summary>
        /// Get all nested types.
        /// </summary>
        public ITypeCollection Types
        {
            get { return _types ?? (_types = new TypeCollection(this)); }
			set { _types = value; }
        }
        private ITypeCollection _types;

	    #region IFormattable Members
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return SyntaxFormatter.Format(this, format, formatProvider);
        }
        #endregion

        #region Object Override Members
        public override bool Equals(object obj)
        {
            return this.IsEqual(obj as IType);
        }

        public override int GetHashCode()
        {
            string s = FullName;
            if (s != null)
                return s.GetHashCode();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
        #endregion

        #region ICodeNode Members

	    public override IEnumerable<ICodeNode> ChildNodes
        {
            get { return new ICodeNode[] { Types, Fields, Properties, Events, Methods }; }
        }
        #endregion

        #region Names

        /// <summary>
        /// Gets c# keyword used for this type
        /// </summary>
        public string CSharpKeyword
        {
            get
            {
                var st = this.SystemType();
                return st != null ? st.CSharpKeyword : "";
            }
        }

        /// <summary>
        /// Gets unique key of this type. Used for <see cref="TypeFactory"/>.
        /// </summary>
        public virtual string Key
        {
            get { return FullName; }
        }

        /// <summary>
        /// Gets name of the type used in signatures.
        /// </summary>
        public virtual string SigName
        {
            get
            {
                if (_sigName == null)
                {
                	string k = CSharpKeyword;
                	_sigName = !string.IsNullOrEmpty(k) ? k : GenericType.ToDisplayName(FullName, true);
                }
            	return _sigName;
            }
        }
        private string _sigName;

        /// <summary>
        /// Name with names of enclosing types.
        /// </summary>
        public virtual string NestedName
        {
            get { return _nestedName ?? (_nestedName = GetName(this, false)); }
        }
        private string _nestedName;

        #endregion

        #region XmlSerialization

	    #endregion

        #region Name Utils
        public static string GetName(IType type, TypeNameKind kind)
        {
            switch (kind)
            {
                case TypeNameKind.DisplayName:
                    return type.DisplayName;
                case TypeNameKind.FullName:
                    return type.FullName;
                case TypeNameKind.Key:
                    return type.Key;
                case TypeNameKind.Name:
                    return type.Name;
                case TypeNameKind.NestedName:
                    return type.NestedName;
                case TypeNameKind.SigName:
                    return type.SigName;
                case TypeNameKind.CSharpKeyword:
                    return type.CSharpKeyword;
            }
            return type.FullName;
        }

        public static string Format(StringBuilder sb, IEnumerable<IType> types, Converter<IType, string> getName,
            string prefix, string suffix, string sep)
        {
            if (prefix != null)
                sb.Append(prefix);
            bool s = false;
            foreach (var type in types)
            {
                if (s) sb.Append(sep);
                sb.Append(getName(type));
                s = true;
            }
            if (suffix != null)
                sb.Append(suffix);
            return sb.ToString();
        }

        public static string Format(IEnumerable<IType> types, Converter<IType, string> getName, string prefix, string suffix, string sep)
        {
            var sb = new StringBuilder();
            Format(sb, types, getName, prefix, suffix, sep);
            return sb.ToString();
        }

        public static string Format(IEnumerable<IType> types, Converter<IType, string> getName, string sep)
        {
            return Format(types, getName, null, null, sep);
        }

	    public static string GetName(IType type, bool withNamespace)
        {
            var dt = type.DeclaringType;
            if (dt == null)
            {
                if (withNamespace)
                {
                    string ns = type.Namespace;
                    if (!string.IsNullOrEmpty(ns))
                        return ns + "." + type.Name;
                }
                return type.Name;
            }
            return GetName(dt, withNamespace) + "+" + type.Name;
        }

        public static string GetKeyword(string lang, IType type)
        {
            var st = type.SystemType();
            if (st != null)
            {
                string name = st.Code.EnumString(lang);
                if (!string.IsNullOrEmpty(name))
                    return name;
            }
            return "";
        }
        #endregion
    }
}