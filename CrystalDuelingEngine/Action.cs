using System.Collections.Generic;
using System.Linq;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine
{
	public sealed class Action : IRenderable, ISerializable
	{
		public Action(string key, string name, IEnumerable<TagBase> tags)
		{
			Key = key;
			Name = name;
			Tags = new TagCollection(tags, this);
		}

		public string SerializationName => nameof(Action);

		public string Name { get; }

		public string Key { get; }

		public TagCollection Tags { get; }

		public Action Clone()
		{
			return new Action(this);
		}

		public string RenderForLog()
		{
			return $"{Name} ({Key})";
		}

		public string RenderForUi()
		{
			return Name;
		}

		public void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Name), Name);
			serializer.SerializeValue(nameof(Key), Key);
			serializer.SerializeValue(nameof(Tags), Tags);

			serializer.EndObject();
		}

		private Action(IDeserializer deserializer)
		{
			Key = deserializer.GetValue<string>(nameof(Key));
			Name = deserializer.GetValue<string>(nameof(Name));
			Tags = new TagCollection(deserializer.GetValue<IEnumerable<TagBase>>(nameof(Tags)), this);
		}

		private Action(Action that)
		{
			Key = that.Key;
			Name = that.Name;
			Tags = new TagCollection(that.Tags.Select(x => x.Clone()), this);
		}

		static Action()
		{
			SerializationManager.RegisterSerializable(nameof(Action), x => new Action(x));
		}
	}
}
