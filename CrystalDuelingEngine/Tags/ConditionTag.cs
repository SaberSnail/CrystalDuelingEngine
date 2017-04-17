using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Tags
{
	public sealed class ConditionTag : ConditionTagBase
	{
		public ConditionTag(string key, ConditionBase condition)
			: this(key, condition, null)
		{
		}

		public ConditionTag(string key, ConditionBase condition, int? duration)
			: base(key, condition, duration)
		{
		}

		public override string SerializationName => nameof(ConditionTag);

		public override TagBase Clone()
		{
			return new ConditionTag(this);
		}

		public override TagBase CloneWithKey(string key)
		{
			return new ConditionTag(this, key);
		}

		public override TagBase CloneWithDuration(int? duration)
		{
			return new ConditionTag(this, duration);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		private ConditionTag(ConditionTag that)
			: base(that)
		{
		}

		private ConditionTag(ConditionTag that, string key)
			: base(that, key)
		{
		}

		private ConditionTag(ConditionTag that, int? duration)
			: base(that, duration)
		{
		}

		private ConditionTag(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static ConditionTag()
		{
			SerializationManager.RegisterSerializable(nameof(ConditionTag), x => new ConditionTag(x));
		}
	}
}
