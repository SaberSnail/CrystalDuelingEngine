using System;
using CrystalDuelingEngine.Serialization;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Tags
{
	public sealed class StringValueTag : ValueTagBase, IAddableTag, IEquatable<StringValueTag>, IComparable<StringValueTag>
	{
		public StringValueTag(string key, string value)
			: this(key, value, null)
		{
		}

		public StringValueTag(string key, string value, int? duration)
			: base(key, duration)
		{
			Value = value;
		}

		public override string SerializationName => nameof(StringValueTag);

		public override TagBase Clone()
		{
			return new StringValueTag(this);
		}

		public override TagBase CloneWithKey(string key)
		{
			return new StringValueTag(this, key);
		}

		public override TagBase CloneWithDuration(int? duration)
		{
			return new StringValueTag(this, duration);
		}

		public string Value { get; }

		public IAddableTag AddTag(IAddableTag tag)
		{
			if (tag == null)
				throw new ArgumentNullException(nameof(tag));

			StringValueTag that = tag as StringValueTag;
			if (that == default(StringValueTag))
				throw new InvalidRulesException($"Tried to add tags of different types '{RenderForLog()}' and '{tag.RenderForLog()}'.");
			return new StringValueTag(Key, Value + that.Value, Duration);
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
			return Value;
		}

		public override string RenderForLog()
		{
			return $"{Key} '{Value}'";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ValueTagBase);
		}

		public override bool Equals(ValueTagBase tag)
		{
			if (ReferenceEquals(tag, null))
				return false;

			StringValueTag stringTag = tag as StringValueTag;
			return ReferenceEquals(stringTag, null) ? Equals(tag.GetValueAsString()) : Equals(stringTag);
		}

		public bool Equals(StringValueTag tag)
		{
			return Equals(ReferenceEquals(tag, null) ? null : tag.Value);
		}

		public override int CompareTo(ValueTagBase tag)
		{
			if (ReferenceEquals(tag, null))
				return 1;

			StringValueTag stringTag = tag as StringValueTag;
			string tagValue = ReferenceEquals(stringTag, null) ? tag.GetValueAsString() : stringTag.Value;
			return string.Compare(Value, tagValue, StringComparison.CurrentCultureIgnoreCase);
		}

		public int CompareTo(StringValueTag tag)
		{
			return string.Compare(Value, ReferenceEquals(tag, null) ? null : tag.Value, StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool operator ==(StringValueTag left, ValueTagBase right)
		{
			return ObjectImpl.OperatorEquality(left, right);
		}

		public static bool operator !=(StringValueTag left, ValueTagBase right)
		{
			return ObjectImpl.OperatorInequality(left, right);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private StringValueTag(StringValueTag that)
			: base(that)
		{
			Value = that.Value;
		}

		private StringValueTag(StringValueTag that, string key)
			: base(that, key)
		{
			Value = that.Value;
		}

		private StringValueTag(StringValueTag that, int? duration)
			: base(that, duration)
		{
			Value = that.Value;
		}

		private StringValueTag(IDeserializer deserializer)
			: base(deserializer)
		{
			Value = deserializer.GetValue<string>(nameof(Value));
		}

		static StringValueTag()
		{
			SerializationManager.RegisterSerializable(nameof(StringValueTag), x => new StringValueTag(x));
		}
	}
}
