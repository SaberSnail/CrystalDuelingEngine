using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrystalDuelingEngine.Effects;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.Tags;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine
{
	public sealed class Result : IRenderable, ISerializable
	{
		public Result(string key, string name, IEnumerable<EffectBase> effects)
			: this(key, name, effects, null)
		{
		}

		public Result(string key, string name, IEnumerable<EffectBase> effects, IEnumerable<TagBase> tags)
		{
			Key = key;
			Name = name;
			Effects = effects.ToList().AsReadOnly();
			Tags = new TagCollection(tags.EmptyIfNull(), this);
		}

		public string SerializationName => nameof(Result);

		public string Name { get; }

		public string Key { get; }

		public ReadOnlyCollection<EffectBase> Effects { get; }

		public TagCollection Tags { get; }

		public string RenderForLog()
		{
			return $"{Name} ({Key})";
		}

		public string RenderForUi()
		{
			return $"{Name} ({Key})";
		}

		public void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Name), Name);
			serializer.SerializeValue(nameof(Key), Key);
			serializer.SerializeValue(nameof(Effects), Effects);
			serializer.SerializeValue(nameof(Tags), Tags);

			serializer.EndObject();
		}

		private Result(IDeserializer deserializer)
		{
			Name = deserializer.GetValue<string>(nameof(Name));
			Key = deserializer.GetValue<string>(nameof(Key));
			Effects = deserializer.GetValues<EffectBase>(nameof(Effects)).EmptyIfNull().ToList().AsReadOnly();
			Tags = new TagCollection(deserializer.GetValue<IEnumerable<TagBase>>(nameof(Tags)), this);
		}

		static Result()
		{
			SerializationManager.RegisterSerializable(nameof(Result), x => new Result(x));
		}
	}
}
