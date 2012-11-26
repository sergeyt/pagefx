using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataDynamics.PageFX.CodeModel.Syntax;
using Enumerable = System.Linq.Enumerable;

namespace DataDynamics.PageFX.CodeModel
{
	public class MethodCollection : MultiMemberCollection<IMethod>, IMethodCollection
    {
        public MethodCollection(IType owner) : base(owner)
        {
        }

    	protected override void OnAdd(IMethod method)
        {
            if (method.IsConstructor)
            {
                _ctors.Add(method);
                if (method.IsStatic)
                    StaticConstructor = method;
            }
        }

        public IEnumerable<IMethod> Constructors
        {
            get { return _ctors; }
        }
        private readonly List<IMethod> _ctors = new List<IMethod>();

    	public IMethod StaticConstructor { get; private set; }

    	public CodeNodeType NodeType
        {
            get { return CodeNodeType.Methods; }
        }

        public object Tag { get; set; }

    	public string ToString(string format, IFormatProvider formatProvider)
        {
            return SyntaxFormatter.Format(this, format, formatProvider);
        }

    	public override string ToString()
        {
            return ToString(null, null);
        }

    	public static readonly IMethodCollection Empty = new EmptyMethodCollection();

		private sealed class EmptyMethodCollection : IMethodCollection
		{
			public int Count
			{
				get { return 0; }
			}

			public IMethod this[int index]
			{
				get { return null; }
			}

			public void Add(IMethod method)
			{
			}

			public IEnumerable<IMethod> Find(string name)
			{
				return Enumerable.Empty<IMethod>();
			}

			public IEnumerable<IMethod> Constructors
			{
				get { return Enumerable.Empty<IMethod>(); }
			}

			public IMethod StaticConstructor
			{
				get { return null; }
			}

			public IEnumerator<IMethod> GetEnumerator()
			{
				return Enumerable.Empty<IMethod>().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public CodeNodeType NodeType
			{
				get { return CodeNodeType.Methods; }
			}

			public IEnumerable<ICodeNode> ChildNodes
			{
				get { return this.Cast<ICodeNode>(); }
			}

			public object Tag { get; set; }

			public string ToString(string format, IFormatProvider formatProvider)
			{
				return "";
			}
		}
    }
}