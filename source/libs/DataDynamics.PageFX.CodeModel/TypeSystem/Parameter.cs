using System;
using System.Collections.Generic;
using DataDynamics.PageFX.CodeModel.Syntax;

namespace DataDynamics.PageFX.CodeModel.TypeSystem
{
    public sealed class Parameter : CustomAttributeProvider, IParameter
    {
	    public Parameter()
        {
        }

        public Parameter(IType type, string name)
        {
            Type = type;
            Name = name;
        }

        public Parameter(IType type, string name, int index)
        {
            Type = type;
            Name = name;
            Index = index;
        }

	    /// <summary>
        /// Gets or sets param attributes.
        /// </summary>
        public ParamAttributes Flags { get; set; }

	    public int Index { get; set; }

	    /// <summary>
	    /// Gets or sets param type.
	    /// </summary>
	    public IType Type { get; set; }

	    /// <summary>
	    /// Gets or sets param name
	    /// </summary>
	    public string Name { get; set; }

	    /// <summary>
	    ///  Gets or sets param value
	    /// </summary>
	    public object Value { get; set; }

	    /// <summary>
        /// Gets the flag indicating whether parameter is passed by reference.
        /// </summary>
        public bool IsByRef
        {
            get { return Type != null && Type.TypeKind == TypeKind.Reference; }
        }

        public bool IsIn
        {
            get { return (Flags & ParamAttributes.In) != 0; }
        }

        public bool IsOut
        {
            get { return (Flags & ParamAttributes.Out) != 0; }
        }

        /// <summary>
        /// Gets or sets flags indicating whether the method parameter that takes an argument where the number of arguments is variable.
        /// </summary>
        public bool HasParams { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether address of this parameter used onto the evaluation stack.
        /// </summary>
        public bool IsAddressed { get; set; }

        public IInstruction Instruction { get; set; }

        public CodeNodeType NodeType
        {
            get { return CodeNodeType.Parameter; }
        }

        public IEnumerable<ICodeNode> ChildNodes
        {
            get { return null; }
        }

    	/// <summary>
    	/// Gets or sets user defined data assotiated with this object.
    	/// </summary>
    	public object Tag { get; set; }

	    public string ToString(string format, IFormatProvider formatProvider)
        {
            return SyntaxFormatter.Format(this, format, formatProvider);
        }

	    /// <summary>
	    /// Gets or sets documentation of this member
	    /// </summary>
	    public string Documentation { get; set; }

	    public object Clone()
	    {
		    return new Parameter(Type, Name, Index)
			    {
				    Documentation = Documentation,
				    Flags = Flags,
				    HasParams = HasParams,
				    Value = Clone(Value),
				    IsAddressed = IsAddressed
			    };
	    }

	    private static object Clone(object obj)
        {
            var c = obj as ICloneable;
            if (c != null)
                return c.Clone();
            return obj;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
    }
}