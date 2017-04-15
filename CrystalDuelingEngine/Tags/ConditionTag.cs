using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;

namespace CrystalDuelingEngine.Tags
{
	public class ConditionTag : TagBase
	{
		public ConditionTag(string key, ConditionBase condition)
			: this(key, condition, null)
		{
		}

		public ConditionTag(string key, ConditionBase condition, int? duration)
			: base(key, duration)
		{
			Condition = condition;
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

		public override string RenderForLog()
		{
			return $"{Key} : {Condition.RenderForLog()}";
		}

		public ConditionBase Condition { get; }

		public bool IsTrue(AttackState attackState)
		{
			return Condition == null || Condition.IsTrue(attackState);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Condition), Condition);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected ConditionTag(ConditionTag that)
			: base(that)
		{
			Condition = that.Condition;
		}

		protected ConditionTag(ConditionTag that, string key)
			: base(that, key)
		{
			Condition = that.Condition;
		}

		protected ConditionTag(ConditionTag that, int? duration)
			: base(that, duration)
		{
			Condition = that.Condition;
		}

		protected ConditionTag(IDeserializer deserializer)
			: base(deserializer)
		{
			Condition = deserializer.GetValue<ConditionBase>(nameof(Condition));
		}

		static ConditionTag()
		{
			SerializationManager.RegisterSerializable(nameof(ConditionTag), x => new ConditionTag(x));
		}
	}
}
