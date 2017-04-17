using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;

namespace CrystalDuelingEngine.Tags
{
	public abstract class ConditionTagBase : TagBase
	{
		public ConditionBase Condition { get; }

		public override string RenderForLog()
		{
			return $"{Key} : {Condition.RenderForLog()}";
		}

		public bool IsTrue(AttackState attackState)
		{
			return Condition == null || Condition.IsTrue(attackState);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.SerializeValue(nameof(Condition), Condition);
			base.Serialize(serializer);
		}

		protected ConditionTagBase(string key, ConditionBase condition, int? duration)
			: base(key, duration)
		{
			Condition = condition;
		}

		protected ConditionTagBase(ConditionTagBase that)
			: base(that)
		{
			Condition = that.Condition;
		}

		protected ConditionTagBase(ConditionTagBase that, string key)
			: base(that, key)
		{
			Condition = that.Condition;
		}

		protected ConditionTagBase(ConditionTagBase that, int? duration)
			: base(that, duration)
		{
			Condition = that.Condition;
		}

		protected ConditionTagBase(IDeserializer deserializer)
			: base(deserializer)
		{
			Condition = deserializer.GetValue<ConditionBase>(nameof(Condition));
		}
	}
}
