namespace CrystalDuelingEngine.Serialization
{
	public interface ISerializer
	{
		void StartObject(ISerializable obj);
		void EndObject();

		void SerializeValue(string key, object value);
	}
}
