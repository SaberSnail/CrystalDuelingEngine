using System.Collections.Generic;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;

namespace CrystalDuelingEngine.Conditions
{
	public sealed class AlwaysCondition : ConditionBase
	{
		public static readonly AlwaysCondition True = new AlwaysCondition(true);
		public static readonly AlwaysCondition False = new AlwaysCondition(false);

		public override string SerializationName => nameof(AlwaysCondition);

		public bool Value { get; }

		public override bool IsTrue(AttackState state)
		{
			return Value;
		}

		public override string RenderForLog()
		{
			return $"always {Value}";
		}

		public override string RenderForUi()
		{
			return $"always {Value}";
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);
			serializer.SerializeValue(nameof(Value), Value);

			serializer.EndObject();
		}

		protected override bool IsValidCore(List<string> errors)
		{
			return true;
		}

		private AlwaysCondition(bool value)
		{
			Value = value;
		}

		private AlwaysCondition(IDeserializer deserializer)
			: base(deserializer)
		{
			Value = deserializer.GetValue<bool>(nameof(Value));
		}

		static AlwaysCondition()
		{
			SerializationManager.RegisterSerializable(nameof(AlwaysCondition), x => new AlwaysCondition(x));
		}
	}
}
