using System;
using System.Collections.Generic;

namespace DataDynamics.PageFX.CodeModel
{
	//TODO: Consider to rename to ConstructedType

    public abstract class CompoundType : ICompoundType
    {
        #region Constructors

    	protected CompoundType()
        {
        }

    	protected CompoundType(IType elementType)
        {
            ElementType = elementType;
        }

        #endregion

        #region ICompoundType Members
        public IType ElementType
        {
            get { return _elementType; }
            set { _elementType = value; }
        }
        IType _elementType;
        #endregion

        #region IType Members
        public string Namespace
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Namespace;
                return null;
            }
            set
            {
                if (_elementType != null)
                    _elementType.Namespace = value;
            }
        }

        public string FullName
        {
            get { return _fullName ?? (_fullName = ElementType.FullName + NameSuffix); }
        }
        private string _fullName;

        public abstract TypeKind TypeKind { get; }

        public bool IsAbstract
        {
            get { return _elementType != null && _elementType.IsAbstract; }
        	set
            {
                if (_elementType != null)
                    _elementType.IsAbstract = value;
            }
        }

        public bool IsSealed
        {
            get { return _elementType != null && _elementType.IsSealed; }
        	set
            {
                if (_elementType != null)
                    _elementType.IsSealed = value;
            }
        }

        public bool IsBeforeFieldInit
        {
            get { return _elementType != null && _elementType.IsBeforeFieldInit; }
        	set
            {
                if (_elementType != null)
                    _elementType.IsBeforeFieldInit = value;
            }
        }

        /// <summary>
        /// Gets or sets the flag indicating wheher the type is generated by compiler.
        /// </summary>
        public bool IsCompilerGenerated
        {
            get { return _elementType != null && _elementType.IsCompilerGenerated; }
        	set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        /// <summary>
        /// Determines whether this type is class.
        /// </summary>
        public bool IsClass
        {
            get { return false; }
        }

        public bool IsArray
        {
            get { return TypeKind == TypeKind.Array; }
        }

        /// <summary>
        /// Determines whether this type is enum type.
        /// </summary>
        public bool IsEnum
        {
            get { return false; }
        }

        public bool HasIEnumerableInstance { get; set; }

        public IMethod DeclaringMethod
        {
            get
            {
                if (_elementType != null)
                    return _elementType.DeclaringMethod;
                return null;
            }
            set
            {
                if (_elementType != null)
                    _elementType.DeclaringMethod = value;
            }
        }

        public virtual IType BaseType
        {
            get
            {
                if (_elementType != null)
                    return _elementType.BaseType;
                return null;
            }
            set
            {
                if (_elementType != null)
                    _elementType.BaseType = value;
            }
        }

        public virtual ITypeCollection Interfaces
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Interfaces;
                return null;
            }
        }

        public virtual ITypeCollection Types
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Types;
                return null;
            }
        }

        public virtual IFieldCollection Fields
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Fields;
                return null;
            }
        }

        public virtual IMethodCollection Methods
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Methods;
                return null;
            }
        }

        public virtual IPropertyCollection Properties
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Properties;
                return null;
            }
        }

        public virtual IEventCollection Events
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Events;
                return null;
            }
        }

        public virtual ITypeMemberCollection Members
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Members;
                return null;
            }
        }

        public IType ValueType
        {
            get
            {
                if (_elementType != null)
                    return _elementType.ValueType;
                return null;
            }
        }

        public SystemType SystemType
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public virtual ClassLayout Layout
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Layout;
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets members defined with syntax of some language
        /// </summary>
        public string CustomMembers
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets type source code.
        /// </summary>
        public string SourceCode
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets c# keyword used for this type
        /// </summary>
        public string CSharpKeyword
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets unique key of this type. Used for <see cref="TypeFactory"/>.
        /// </summary>
        public string Key
        {
            get { return _key ?? (_key = ElementType.Key + NameSuffix); }
        }
        private string _key;
        
        /// <summary>
        /// Gets name of the type used in signatures.
        /// </summary>
        public string SigName
        {
            get { return _sigName ?? (_sigName = ElementType.SigName + SigSuffix); }
        }
        private string _sigName;

        protected abstract string SigSuffix { get; }

        /// <summary>
        /// Name with names of enclosing types.
        /// </summary>
        public string NestedName
        {
            get { return _elementType.NestedName + NameSuffix; }
        }
        #endregion

        #region ITypeMember Members
        /// <summary>
        /// Gets the assembly in which the type is declared.
        /// </summary>
        public IAssembly Assembly
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Assembly;
                return null;
            }
        }

        /// <summary>
        /// Gets the module in which the current type is defined. 
        /// </summary>
        public IModule Module
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Module;
                return null;
            }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the kind of this member.
        /// </summary>
        public TypeMemberType MemberType
        {
            get { return TypeMemberType.Type; }
        }

        public virtual string NameSuffix
        {
            get { return ""; }
        }

        public string Name
        {
            get { return _name ?? (_name = ElementType.Name + NameSuffix); }
        	set { throw new NotSupportedException(); }
        }
        private string _name;

        public string DisplayName
        {
            get { return _displayName ?? (_displayName = ElementType.DisplayName + NameSuffix); }
        }
        private string _displayName;

        public IType DeclaringType
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public IType Type
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets visibility of this member.
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                if (_elementType != null)
                    return _elementType.Visibility;
                return Visibility.Private;
            }
            set
            {
                if (_elementType != null)
                    _elementType.Visibility = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                if (_elementType != null)
                    return _elementType.IsVisible;
                return false;
            }
        }

        public bool IsStatic
        {
            get
            {
                if (_elementType != null)
                    return _elementType.IsStatic;
                return false;
            }
            set
            {
                if (_elementType != null)
                    _elementType.IsStatic = value;
            }
        }

        public bool IsSpecialName
        {
            get
            {
                if (_elementType != null)
                    return _elementType.IsSpecialName;
                return false;
            }
            set { throw new NotSupportedException(); }
        }

        public bool IsRuntimeSpecialName
        {
            get
            {
                if (_elementType != null)
                    return _elementType.IsRuntimeSpecialName;
                return false;
            }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets value that identifies a metadata element. 
        /// </summary>
        public int MetadataToken
        {
            get { return -1; }
            set { throw new NotSupportedException(); }
        }
        #endregion

        #region ICustomAttributeProvider Members
        public ICustomAttributeCollection CustomAttributes
        {
            get
            {
                if (_elementType != null)
                    return _elementType.CustomAttributes;
                return null;
            }
        }
        #endregion

        #region ICodeNode Members

        public CodeNodeType NodeType
        {
            get { return CodeNodeType.Type; }
        }

        public IEnumerable<ICodeNode> ChildNodes
        {
            get { return CMHelper.Enumerate(_elementType); }
        }

    	/// <summary>
    	/// Gets or sets user defined data assotiated with this object.
    	/// </summary>
    	public object Tag { get; set; }

    	#endregion

        #region IFormattable Members
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return FullName;
        }
        #endregion

        #region IDocumentationProvider Members

    	/// <summary>
    	/// Gets or sets documentation of this member
    	/// </summary>
    	public string Documentation { get; set; }

    	#endregion

        #region Object Override Members
        public override bool Equals(object obj)
        {
            return CMHelper.AreEquals(this, obj as IType);
        }

        public override int GetHashCode()
        {
            return CMHelper.GetHashCode(this);
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
        #endregion
    }

    public sealed class PointerType : CompoundType
    {
        public PointerType(IType elementType)
            : base(elementType)
        {
        }

        public override TypeKind TypeKind
        {
            get { return TypeKind.Pointer; }
        }

        public override string NameSuffix
        {
            get { return "*"; }
        }

        protected override string SigSuffix
        {
            get { return "_ptr"; }
        }
    }

    public sealed class ReferenceType : CompoundType
    {
        public ReferenceType(IType elementType)
            : base(elementType)
        {
        }

        public override TypeKind TypeKind
        {
            get { return TypeKind.Reference; }
        }

        public override string NameSuffix
        {
            get { return "&"; }
        }

        protected override string SigSuffix
        {
            get { return "_ref"; }
        }
    }
}