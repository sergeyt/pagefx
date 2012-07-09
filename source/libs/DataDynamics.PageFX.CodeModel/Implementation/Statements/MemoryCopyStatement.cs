using System;
using System.Collections.Generic;

namespace DataDynamics.PageFX.CodeModel
{
    public class MemoryCopyStatement : Statement, IMemoryCopyStatement
    {
    	public IExpression Destination { get; set; }

    	public IExpression Source { get; set; }

    	public IExpression Size { get; set; }

    	public override IEnumerable<ICodeNode> ChildNodes
        {
            get { return new ICodeNode[] { Destination, Source, Size }; }
        }

    	public override bool Equals(object obj)
        {
            if (obj == this) return true;
            var s = obj as IMemoryCopyStatement;
            if (s == null) return false;
            if (!Equals(s.Destination, Destination)) return false;
            if (!Equals(s.Source, Source)) return false;
            if (!Equals(s.Size, Size)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return new []{Destination, Source, Size}.EvalHashCode();
        }
    }
}