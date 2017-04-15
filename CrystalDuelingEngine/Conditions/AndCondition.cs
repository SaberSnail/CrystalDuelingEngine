using System.Collections.Generic;
using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Conditions
{
	public sealed class AndCondition : BinaryLogicCondition
	{
		public AndCondition(IEnumerable<ConditionBase> children)
			: base(children)
		{
		}

		public override string SerializationName => nameof(AndCondition);

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override bool CombineResult(bool? current, bool result)
		{
			return current.GetValueOrDefault(true) && result;
		}

		protected override string RenderOperatorForLog()
		{
			return "AND";
		}

		protected override string RenderOperatorForUi()
		{
			return "AND";
		}

		private AndCondition(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static AndCondition()
		{
			SerializationManager.RegisterSerializable(nameof(AndCondition), x => new AndCondition(x));
		}
	}
}
