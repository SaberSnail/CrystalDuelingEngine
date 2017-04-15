using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Tags
{
	public sealed class SimpleTag : TagBase
	{
		public SimpleTag(string key)
			: this(key, null)
		{
		}

		public SimpleTag(string key, int? duration)
			: base(key, duration)
		{
		}

		public override string SerializationName => nameof(SimpleTag);

		public override TagBase Clone()
		{
			return new SimpleTag(this);
		}

		public override TagBase CloneWithKey(string key)
		{
			return new SimpleTag(this, key);
		}

		public override TagBase CloneWithDuration(int? duration)
		{
			return new SimpleTag(this, duration);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		private SimpleTag(SimpleTag that)
			: base(that)
		{
		}

		private SimpleTag(SimpleTag that, string key)
			: base(that, key)
		{
		}

		private SimpleTag(SimpleTag that, int? duration)
			: base(that, duration)
		{
		}

		private SimpleTag(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static SimpleTag()
		{
			SerializationManager.RegisterSerializable(nameof(SimpleTag), x => new SimpleTag(x));
		}
	}
}
