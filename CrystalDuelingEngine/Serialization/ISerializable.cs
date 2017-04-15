namespace CrystalDuelingEngine.Serialization
{
	public interface ISerializable
	{
		string SerializationName { get; }
		void Serialize(ISerializer serializer);
	}
}
