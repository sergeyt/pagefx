using System;
using System.Collections.Generic;
using DataDynamics.PageFX.Common.CodeModel;
using DataDynamics.PageFX.Common.Services;

namespace DataDynamics.PageFX.Common.TypeSystem
{
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

    	public IType ElementType { get; set; }

    	#endregion

        #region IType Members
        public string Namespace
        {
            get { return ElementType != null ? ElementType.Namespace : null; }
        	set
            {
                if (ElementType != null)
                    ElementType.Namespace = value;
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
            get { return ElementType != null && ElementType.IsAbstract; }
        	set
            {
                if (ElementType != null)
                    ElementType.IsAbstract = value;
            }
        }

        public bool IsSealed
        {
            get { return ElementType != null && ElementType.IsSealed; }
        	set
            {
                if (ElementType != null)
                    ElementType.IsSealed = value;
            }
        }

        public bool IsBeforeFieldInit
        {
            get { return ElementType != null && ElementType.IsBeforeFieldInit; }
        	set
            {
                if (ElementType != null)
                    ElementType.IsBeforeFieldInit = value;
            }
        }

        /// <summary>
        /// Gets or sets the flag indicating wheher the type is generated by compiler.
        /// </summary>
        public bool IsCompilerGenerated
        {
            get { return ElementType != null && ElementType.IsCompilerGenerated; }
        	set { throw new NotSupportedException(); }
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
            get { return ElementType != null ? ElementType.DeclaringMethod : null; }
        	set
            {
                if (ElementType != null)
                    ElementType.DeclaringMethod = value;
            }
        }

        public virtual IType BaseType
        {
            get { return ElementType != null ? ElementType.BaseType : null; }
        	set
            {
                if (ElementType != null)
                    ElementType.BaseType = value;
            }
        }

        public virtual ITypeCollection Interfaces
        {
            get { return ElementType != null ? ElementType.Interfaces : null; }
        }

        public virtual ITypeCollection Types
        {
            get { return ElementType != null ? ElementType.Types : null; }
        }

        public virtual IFieldCollection Fields
        {
            get { return ElementType != null ? ElementType.Fields : null; }
        }

        public virtual IMethodCollection Methods
        {
            get { return ElementType != null ? ElementType.Methods : null; }
        }

        public virtual IPropertyCollection Properties
        {
            get { return ElementType != null ? ElementType.Properties : null; }
        }

        public virtual IEventCollection Events
        {
            get { return ElementType != null ? ElementType.Events : null; }
        }

        public virtual ITypeMemberCollection Members
        {
            get { return ElementType != null ? ElementType.Members : null; }
        }

        public IType ValueType
        {
            get { return ElementType != null ? ElementType.ValueType : null; }
        }

        public virtual ClassLayout Layout
        {
            get { return ElementType != null ? ElementType.Layout : null; }
        	set { throw new NotSupportedException(); }
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
            get { return ElementType.NestedName + NameSuffix; }
        }
        #endregion

        #region ITypeMember Members
        /// <summary>
        /// Gets the assembly in which the type is declared.
        /// </summary>
        public IAssembly Assembly
        {
            get { return ElementType != null ? ElementType.Assembly : null; }
        }

        /// <summary>
        /// Gets the module in which the current type is defined. 
        /// </summary>
        public IModule Module
        {
            get { return ElementType != null ? ElementType.Module : null; }
        	set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the kind of this member.
        /// </summary>
        public MemberType MemberType
        {
            get { return MemberType.Type; }
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
            get { return null; }
            set { throw new NotSupportedException(); }
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
            get { return ElementType != null ? ElementType.Visibility : Visibility.Private; }
        	set
            {
                if (ElementType != null)
                    ElementType.Visibility = value;
            }
        }

        public bool IsVisible
        {
            get { return ElementType != null && ElementType.IsVisible; }
        }

        public bool IsStatic
        {
            get { return ElementType != null && ElementType.IsStatic; }
        	set
            {
                if (ElementType != null)
                    ElementType.IsStatic = value;
            }
        }

        public bool IsSpecialName
        {
            get { return ElementType != null && ElementType.IsSpecialName; }
        	set { throw new NotSupportedException(); }
        }

        public bool IsRuntimeSpecialName
        {
            get { return ElementType != null && ElementType.IsRuntimeSpecialName; }
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
            get { return ElementType != null ? ElementType.CustomAttributes : null; }
        }

        #endregion

        #region ICodeNode Members

	    public IEnumerable<ICodeNode> ChildNodes
        {
            get { return new ICodeNode[] {ElementType}; }
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
            return this.IsEqual(obj as IType);
        }

        public override int GetHashCode()
        {
            return this.EvalHashCode();
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