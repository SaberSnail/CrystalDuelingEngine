using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine
{
	public sealed class ActionMatrixEntry : ISerializable
	{
		public ActionMatrixEntry(string attackerActionId, string defenderActionId, string resultId)
		{
			AttackerActionId = attackerActionId;
			DefenderActionId = defenderActionId;
			ResultId = resultId;
		}

		public string SerializationName => nameof(ActionMatrixEntry);

		public string AttackerActionId { get; }
		public string DefenderActionId { get; }
		public string ResultId { get; }

		public ActionMatrixEntryKey Key => new ActionMatrixEntryKey(AttackerActionId, DefenderActionId);

		public void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(AttackerActionId), AttackerActionId);
			serializer.SerializeValue(nameof(DefenderActionId), DefenderActionId);
			serializer.SerializeValue(nameof(ResultId), ResultId);

			serializer.EndObject();
		}

		private ActionMatrixEntry(IDeserializer deserializer)
		{
			AttackerActionId = deserializer.GetValue<string>(nameof(AttackerActionId));
			DefenderActionId = deserializer.GetValue<string>(nameof(DefenderActionId));
			ResultId = deserializer.GetValue<string>(nameof(ResultId));
		}

		static ActionMatrixEntry()
		{
			SerializationManager.RegisterSerializable(nameof(ActionMatrixEntry), x => new ActionMatrixEntry(x));
		}
	}
}
