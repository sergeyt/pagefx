using System;
using System.Collections.Generic;
using DataDynamics.PageFX.Common.CodeModel;
using DataDynamics.PageFX.Common.Syntax;

namespace DataDynamics.PageFX.Common.TypeSystem
{
	public abstract class TypeMember : CustomAttributeProvider, ITypeMember
    {
		private IType _type;
	    private IType _declType;

	    #region ITypeMember Members

        /// <summary>
        /// Gets the assembly in which the member is declared.
        /// </summary>
        public IAssembly Assembly
        {
            get
            {
                var mod = Module;
                if (mod != null)
                    return mod.Assembly;
                return null;
            }
        }

        /// <summary>
        /// Gets the module in which the member is defined. 
        /// </summary>
        public virtual IModule Module
        {
            get
            {
                if (DeclaringType != null)
                    return DeclaringType.Module;
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the kind of this member.
        /// </summary>
        public abstract MemberType MemberType { get; }

        /// <summary>
        /// Gets or sets member name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the fullname of the member.
        /// </summary>
        public virtual string FullName
        {
            get 
            {
                var dt = DeclaringType;
                if (dt != null)
                    return dt.FullName + "." + Name;
                return Name;
            }
        }

        public virtual string DisplayName
        {
            get { return Name; }
        }

	    /// <summary>
	    /// Gets or sets the type that declares this member.
	    /// </summary>
	    public IType DeclaringType
	    {
			get { return _declType ?? (_declType = ResolveDeclaringType()); }
			set { _declType = value; }
	    }

	    /// <summary>
	    /// Gets or sets the type of this member (for methods it's return type)
	    /// </summary>
	    public IType Type
	    {
			get { return _type ?? (_type = ResolveType()); }
			set { _type = value; }
	    }

	    protected virtual IType ResolveDeclaringType()
	    {
		    return null;
	    }

	    protected virtual IType ResolveType()
	    {
		    return null;
	    }

	    /// <summary>
        /// Gets visibility of this member.
        /// </summary>
        public virtual Visibility Visibility { get; set; }

        public bool IsVisible
        {
            get
            {
                if (DeclaringType != null)
                {
                    if (!DeclaringType.IsVisible)
                        return false;
                }
                switch(Visibility)
                {
                    case Visibility.Public:
                    case Visibility.NestedPublic:
                        return true;
                }
                return false;
            }
        }

	    internal Modifiers Modifiers { get; set; }

	    internal bool GetModifier(Modifiers mod)
        {
            return (Modifiers & mod) != 0;
        }

        internal void SetModifier(bool value, Modifiers mod)
        {
            if (value) Modifiers |= mod;
            else Modifiers &= ~mod;
        }

        public virtual bool IsStatic
        {
            get { return GetModifier(Modifiers.Static); }
            set { SetModifier(value, Modifiers.Static); }
        }

        public bool IsSpecialName
        {
            get { return GetModifier(Modifiers.SpecialName); }
            set { SetModifier(value, Modifiers.SpecialName); }
        }

        public bool IsRuntimeSpecialName
        {
            get { return GetModifier(Modifiers.RuntimeSpecialName); }
            set { SetModifier(value, Modifiers.RuntimeSpecialName); }
        }

        #endregion

        #region ICodeNode Members

	    public virtual IEnumerable<ICodeNode> ChildNodes
        {
            get { return null; }
        }

    	/// <summary>
    	/// Gets or sets user defined data assotiated with this object.
    	/// </summary>
    	public object Tag { get; set; }

    	#endregion

        #region IFormattable Members
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return SyntaxFormatter.Format(this, format, formatProvider);
        }
        #endregion

        #region IDocumentationProvider Members

    	/// <summary>
    	/// Gets or sets documentation of this member
    	/// </summary>
    	public string Documentation { get; set; }

    	#endregion

        #region Object Override Members
        public override string ToString()
        {
            return ToString(null, null);
        }
        #endregion
    }

	[Flags]
	public enum Modifiers
	{
		None = 0,
		Const = 0x01,
		ReadOnly = 0x02,
		Volatile = 0x04,
		Static = 0x08,
		Extern = 0x10,
		Virtual = 0x20,
		Abstract = 0x40,
		Sealed = 0x80,
		HideBySig = 0x100,
		New = 0x200,
		Unsafe = 0x400,
		HasThis = 0x800,
		ExplicitThis = 0x1000,
		HasDefault = 0x2000,
		BeforeFieldInit = 0x4000,
		SpecialName = 0x8000,
		RuntimeSpecialName = 0x10000,
		EntryPoint = 0x20000,
		CompilerGenerated = 0x40000,
		ExplicitImplementation = 0x80000,
		PInvoke = 0x100000,
	}
}