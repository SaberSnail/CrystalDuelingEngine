using System.Collections.Generic;
using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Conditions
{
	public sealed class OrCondition : BinaryLogicCondition
	{
		public OrCondition(IEnumerable<ConditionBase> children)
			: base(children)
		{
		}

		public override string SerializationName => nameof(OrCondition);

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override bool CombineResult(bool? current, bool result)
		{
			return current.GetValueOrDefault() || result;
		}

		protected override string RenderOperatorForLog()
		{
			return "OR";
		}

		protected override string RenderOperatorForUi()
		{
			return "OR";
		}

		private OrCondition(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static OrCondition()
		{
			SerializationManager.RegisterSerializable(nameof(OrCondition), x => new OrCondition(x));
		}
	}
}
