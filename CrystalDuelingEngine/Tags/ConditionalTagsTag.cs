using System.Collections.Generic;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Tags
{
	public sealed class ConditionalTagsTag : ConditionTagBase
	{
		public ConditionalTagsTag(string key, ConditionBase condition, IEnumerable<TagBase> tags)
			: this(key, condition, tags, null)
		{
		}

		public ConditionalTagsTag(string key, ConditionBase condition, IEnumerable<TagBase> tags, int? duration)
			: base(key, condition, duration)
		{
			Tags = new TagCollection(tags, this);
		}

		public override string SerializationName => nameof(ConditionalTagsTag);

		public TagCollection Tags { get; }

		public override TagBase Clone()
		{
			return new ConditionalTagsTag(this);
		}

		public override TagBase CloneWithKey(string key)
		{
			return new ConditionalTagsTag(this, key);
		}

		public override TagBase CloneWithDuration(int? duration)
		{
			return new ConditionalTagsTag(this, duration);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Tags), Tags);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		private ConditionalTagsTag(ConditionalTagsTag that)
			: base(that)
		{
			Tags = that.Tags;
		}

		private ConditionalTagsTag(ConditionalTagsTag that, string key)
			: base(that, key)
		{
			Tags = that.Tags;
		}

		private ConditionalTagsTag(ConditionalTagsTag that, int? duration)
			: base(that, duration)
		{
			Tags = that.Tags;
		}

		private ConditionalTagsTag(IDeserializer deserializer)
			: base(deserializer)
		{
			Tags = new TagCollection(deserializer.GetValue<IEnumerable<TagBase>>(nameof(Tags)), this);
		}

		static ConditionalTagsTag()
		{
			SerializationManager.RegisterSerializable(nameof(ConditionalTagsTag), x => new ConditionalTagsTag(x));
		}
	}
}
