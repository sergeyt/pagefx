using System;
using System.Linq;
using DataDynamics.PageFX.CodeModel;

namespace DataDynamics.PageFX.CLI.Execution
{
	/// <summary>
	/// HACK: inherited from Exception to allow throw of such objects
	/// </summary>
	internal sealed class Instance : Exception, IFieldStorage
	{
		private readonly VirtualMachine _engine;
		private readonly FieldSlot[] _fields;
		private IMethod _toString;
		private IMethod _equalsMethod;

		public Instance(VirtualMachine engine, Class klass)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			if (klass == null) throw new ArgumentNullException("klass");

			_engine = engine;

			Class = klass;

			if (Type.TypeKind == TypeKind.Delegate)
			{
				_fields = new FieldSlot[3];
				InitFields(_fields.Length);
			}
			else
			{
				_fields = Class.InitFields(klass.Type, false);
			}
		}

		private Instance(VirtualMachine engine, Class klass, FieldSlot[] fields)
		{
			_engine = engine;
			Class = klass;
			_fields = fields;
		}

		private void InitFields(int count)
		{
			for (int i = 0; i < count; i++)
			{
				_fields[i] = new FieldSlot(null);
			}
		}

		public Class Class { get; private set; }

		public IType Type
		{
			get { return Class.Type; }
		}

		public bool IsValueType
		{
			get { return Type.TypeKind == TypeKind.Struct; }
		}

		public FieldSlot[] Fields
		{
			get { return _fields; }
		}

		public override int GetHashCode()
		{
			if (Class.GetHashCodeMethod != null)
			{
				return Convert.ToInt32(_engine.Call(Class.GetHashCodeMethod, new[] { this }));
			}

			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (Class.EqualsMethod != null)
			{
				return Convert.ToBoolean(_engine.Call(Class.EqualsMethod, new[] { this, obj }));
			}

			//TODO: base implementation for value types

			return base.Equals(obj);
		}

		public override string ToString()
		{
			if (Class.ToStringMethod != null)
			{
				return _engine.Call(Class.ToStringMethod, new object[] { this }) as string;
			}
			return Type.ToString();
		}

		public bool IsInstanceOf(IType type)
		{
			if (type.IsInterface)
			{
				return Type.Implements(type);
			}
			return Type.IsSubclassOf(type);
		}

		public Instance Copy()
		{
			var fields = _fields.Select(x => x.Copy()).ToArray();

			return new Instance(_engine, Class, fields);
		}
	}
}