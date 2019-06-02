using System.Collections.Generic;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Conditions
{
	public abstract class UnaryLogicCondition : ConditionBase
	{
		public override string SerializationName => nameof(UnaryLogicCondition);

		public ConditionBase Child { get; }

		public override bool IsTrue(AttackState state)
		{
			return ModifyResult(Child.IsTrue(state));
		}

		public override string RenderForLog()
		{
			return $"{RenderOperatorForLog()} {Child.RenderForLog()}";
		}

		public override string RenderForUi()
		{
			return $"{RenderOperatorForLog()} {Child.RenderForUi()}";
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.SerializeValue(nameof(Child), Child);
			base.Serialize(serializer);
		}

		protected UnaryLogicCondition(ConditionBase child)
		{
			Child = child;
		}

		protected UnaryLogicCondition(IDeserializer deserializer)
			: base(deserializer)
		{
			Child = deserializer.GetValue<ConditionBase>(nameof(Child));
		}

		protected abstract bool ModifyResult(bool result);

		protected abstract string RenderOperatorForLog();

		protected abstract string RenderOperatorForUi();

		protected override bool IsValidCore(List<string> errors)
		{
			if (Child == null)
			{
				errors.Add(OurResources.InvalidConditionMissingChild.FormatCurrentCulture(GetType().Name));
				return false;
			}

			return Child.IsValid(errors);
		}
	}
}
