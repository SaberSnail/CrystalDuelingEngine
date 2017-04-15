using System.Collections.Generic;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;

namespace CrystalDuelingEngine.Conditions
{
	public abstract class ConditionBase : IRenderable, ISerializable
	{
		public abstract string SerializationName { get; }

		public abstract bool IsTrue(AttackState state);

		public bool IsValid(List<string> errors)
		{
			return IsValidCore(errors);
		}

		public abstract string RenderForLog();

		public abstract string RenderForUi();

		public virtual void Serialize(ISerializer serializer)
		{
		}

		protected ConditionBase()
		{
		}

		protected ConditionBase(IDeserializer deserializer)
		{
		}

		protected abstract bool IsValidCore(List<string> errors);
	}
}
