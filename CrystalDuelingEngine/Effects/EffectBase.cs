using System.Collections.Generic;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine.Effects
{
	public abstract class EffectBase : ISerializable
	{
		public abstract string SerializationName { get; }

		public ConditionBase Condition { get; }

		public EffectTarget Target { get; }

		public void ApplyEffects(AttackState state, TagCollection target)
		{
			if (Condition.IsTrue(state))
				ApplyEffectsCore(state, target);
		}

		public bool IsValid(List<string> errors)
		{
			bool isValid = Condition.IsValid(errors);
			return IsValidCore(errors) && isValid;
		}

		protected EffectBase(ConditionBase condition, EffectTarget target)
		{
			Condition = condition;
			Target = target;
		}

		public virtual void Serialize(ISerializer serializer)
		{
			serializer.SerializeValue(nameof(Condition), Condition);
			serializer.SerializeValue(nameof(Target), Target);
		}

		protected EffectBase(IDeserializer deserializer)
		{
			Condition = deserializer.GetValue<ConditionBase>(nameof(Condition));
			Target = deserializer.GetValue<EffectTarget>(nameof(Target));
		}

		protected abstract void ApplyEffectsCore(AttackState state, TagCollection target);

		protected abstract bool IsValidCore(List<string> errors);
	}
}
