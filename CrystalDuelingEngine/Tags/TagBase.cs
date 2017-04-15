using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Tags
{
	public abstract class TagBase : IRenderable, ISerializable
	{
		public abstract string SerializationName { get; }

		public abstract TagBase Clone();

		public abstract TagBase CloneWithKey(string key);

		public abstract TagBase CloneWithDuration(int? duration);

		public string Key { get; }

		public int? Duration { get; }

		public virtual string RenderForLog()
		{
			return Key;
		}

		public string RenderForUi()
		{
			return Key;
		}

		public virtual void Serialize(ISerializer serializer)
		{
			serializer.SerializeValue(nameof(Key), Key);
			serializer.SerializeValue(nameof(Duration), Duration);
		}

		protected TagBase(string key)
			: this(key, null)
		{
		}

		protected TagBase(TagBase that)
			: this(that.Key, that.Duration)
		{
		}

		protected TagBase(TagBase that, string key)
			: this(key, that.Duration)
		{
		}

		protected TagBase(TagBase that, int? duration)
			: this(that.Key, duration)
		{
		}

		protected TagBase(string key, int? duration)
		{
			Key = key;
			Duration = duration;
		}

		protected TagBase(IDeserializer deserializer)
		{
			Key = deserializer.GetValue<string>(nameof(Key));
			Duration = deserializer.GetValue<int?>(nameof(Duration));
		}
	}
}
