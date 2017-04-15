using System;
using System.Globalization;
using CrystalDuelingEngine.Serialization;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Tags
{
	public sealed class IntValueTag : ValueTagBase, IAddableTag, ISubtractableTag, IEquatable<IntValueTag>, IComparable<IntValueTag>
	{
		public IntValueTag(string key, int? value)
			: this(key, value, null)
		{
		}

		public IntValueTag(string key, int? value, int? duration)
			: base(key, duration)
		{
			Value = value;
		}

		public override string SerializationName => nameof(IntValueTag);

		public override TagBase Clone()
		{
			return new IntValueTag(this);
		}

		public override TagBase CloneWithKey(string key)
		{
			return new IntValueTag(this, key);
		}

		public override TagBase CloneWithDuration(int? duration)
		{
			return new IntValueTag(this, duration);
		}

		public int? Value { get; }

		public IAddableTag AddTag(IAddableTag tag)
		{
			if (tag == null)
				throw new ArgumentNullException(nameof(tag));

			IntValueTag that = tag as IntValueTag;
			if (that == null)
				throw new InvalidRulesException($"Tried to add tags of different types '{RenderForLog()}' and '{tag.RenderForLog()}'.");
			return new IntValueTag(Key, Value + that.Value, Duration);
		}

		public ISubtractableTag SubtractTag(ISubtractableTag tag)
		{
			if (tag == null)
				throw new ArgumentNullException(nameof(tag));

			IntValueTag that = tag as IntValueTag;
			if (that == null)
				throw new InvalidRulesException($"Tried to subtract tags of different types '{RenderForLog()}' and '{tag.RenderForLog()}'.");
			return new IntValueTag(Key, Value - that.Value, Duration);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Value), Value);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		public override string GetValueAsString()
		{
			return Value?.ToString(CultureInfo.CurrentUICulture.NumberFormat);
		}

		public override string RenderForLog()
		{
			return $"{Key} ({Value})";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ValueTagBase);
		}

		public override bool Equals(ValueTagBase tag)
		{
			if (ReferenceEquals(tag, null))
				return false;

			IntValueTag intTag = tag as IntValueTag;
			return ReferenceEquals(intTag, null) ? Equals(tag.GetValueAsString()) : Equals(intTag);
		}

		public bool Equals(IntValueTag tag)
		{
			return Equals(ReferenceEquals(tag, null) ? null : tag.Value);
		}

		public override int CompareTo(ValueTagBase tag)
		{
			if (ReferenceEquals(tag, null))
				return 1;

			IntValueTag intTag = tag as IntValueTag;
			int? intValue = ReferenceEquals(intTag, null) ? default(int?) : intTag.Value;
			return Nullable.Compare(Value, intValue);
		}

		public int CompareTo(IntValueTag tag)
		{
			return Nullable.Compare(Value, ReferenceEquals(tag, null) ? null : tag.Value);
		}

		public static bool operator ==(IntValueTag left, ValueTagBase right)
		{
			return ObjectImpl.OperatorEquality(left, right);
		}

		public static bool operator !=(IntValueTag left, ValueTagBase right)
		{
			return ObjectImpl.OperatorInequality(left, right);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private IntValueTag(IntValueTag that)
			: base(that)
		{
			Value = that.Value;
		}

		private IntValueTag(IntValueTag that, string key)
			: base(that, key)
		{
			Value = that.Value;
		}

		private IntValueTag(IntValueTag that, int? duration)
			: base(that, duration)
		{
			Value = that.Value;
		}

		private IntValueTag(IDeserializer deserializer)
			: base(deserializer)
		{
			Value = deserializer.GetValue<int?>(nameof(Value));
		}

		static IntValueTag()
		{
			SerializationManager.RegisterSerializable(nameof(IntValueTag), x => new IntValueTag(x));
		}
	}
}
