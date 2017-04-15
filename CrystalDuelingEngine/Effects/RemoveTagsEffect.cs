using System.Collections.Generic;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine.Effects
{
	public sealed class RemoveTagsEffect : EffectBase
	{
		public RemoveTagsEffect(string tagKey)
			: this(tagKey, MatchKind.Exact, AlwaysCondition.True, EffectTarget.Defender)
		{
		}

		public RemoveTagsEffect(string tagKey, ConditionBase condition)
			: this(tagKey, MatchKind.Exact, condition, EffectTarget.Defender)
		{
		}

		public RemoveTagsEffect(string tagKey, MatchKind tagKeyMatchKind, ConditionBase condition, EffectTarget target)
			: base(condition, target)
		{
			TagKey = tagKey;
			TagKeyMatchKind = tagKeyMatchKind;
		}

		public override string SerializationName => nameof(RemoveTagsEffect);

		public string TagKey { get; }

		public MatchKind TagKeyMatchKind { get; }

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(TagKey), TagKey);
			serializer.SerializeValue(nameof(TagKeyMatchKind), TagKeyMatchKind);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override void ApplyEffectsCore(AttackState state, TagCollection target)
		{
			target.RemoveTags(TagKey, TagKeyMatchKind);
		}

		protected override bool IsValidCore(List<string> errors)
		{
			throw new System.NotImplementedException();
		}

		private RemoveTagsEffect(IDeserializer deserializer)
			: base(deserializer)
		{
			TagKey = deserializer.GetValue<string>(nameof(TagKey));
			TagKeyMatchKind = deserializer.GetValue<MatchKind>(nameof(TagKeyMatchKind));
		}

		static RemoveTagsEffect()
		{
			SerializationManager.RegisterSerializable(nameof(RemoveTagsEffect), x => new RemoveTagsEffect(x));
		}
	}
}
