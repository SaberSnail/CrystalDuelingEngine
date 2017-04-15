using System;
using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Tags
{
	public abstract class ValueTagBase : TagBase, IEquatable<ValueTagBase>, IComparable<ValueTagBase>
	{
		public abstract string GetValueAsString();

		public override bool Equals(object obj)
		{
			return Equals(obj as ValueTagBase);
		}

		public abstract bool Equals(ValueTagBase tag);

		public abstract int CompareTo(ValueTagBase tag);

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		protected ValueTagBase(string key)
			: this(key, null)
		{
		}

		protected ValueTagBase(string key, int? duration)
			: base(key, duration)
		{
		}

		protected ValueTagBase(ValueTagBase that)
			: base(that)
		{
		}

		protected ValueTagBase(ValueTagBase that, string key)
			: base(that, key)
		{
		}

		protected ValueTagBase(ValueTagBase that, int? duration)
			: base(that, duration)
		{
		}

		protected ValueTagBase(IDeserializer deserializer)
			: base(deserializer)
		{
		}
	}
}
