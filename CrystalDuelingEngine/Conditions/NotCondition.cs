using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Conditions
{
	public sealed class NotCondition : UnaryLogicCondition
	{
		public NotCondition(ConditionBase child)
			: base(child)
		{
		}

		public override string SerializationName => nameof(NotCondition);

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override bool ModifyResult(bool result)
		{
			return !result;
		}

		protected override string RenderOperatorForLog()
		{
			return "NOT";
		}

		protected override string RenderOperatorForUi()
		{
			return "NOT";
		}

		private NotCondition(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static NotCondition()
		{
			SerializationManager.RegisterSerializable(nameof(NotCondition), x => new NotCondition(x));
		}
	}
}
