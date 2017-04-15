using System.Collections.Generic;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine.Effects
{
	public sealed class CopyTagsEffect : EffectBase
	{
		public CopyTagsEffect(string tagKey, TagScope tagScope, string newTagKey, KeyConflictResolutionKind conflictResolution)
			: this(tagKey, MatchKind.Exact, tagScope, AlwaysCondition.True, newTagKey, conflictResolution, EffectTarget.Defender)
		{
		}

		public CopyTagsEffect(string tagKey, MatchKind tagKeyMatchKind, TagScope tagScope, ConditionBase condition, string newTagKey, KeyConflictResolutionKind conflictResolution, EffectTarget target)
			: base(condition, target)
		{
			TagKey = tagKey;
			TagKeyMatchKind = tagKeyMatchKind;
			TagScope = tagScope;
			NewTagKey = newTagKey;
			ConflictResolution = conflictResolution;
		}

		public override string SerializationName => nameof(CopyTagsEffect);

		public string TagKey { get; }

		public TagScope TagScope { get; }

		public MatchKind TagKeyMatchKind { get; }

		public string NewTagKey { get; }

		public KeyConflictResolutionKind ConflictResolution { get; }

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(TagKey), TagKey);
			serializer.SerializeValue(nameof(TagScope), TagScope);
			serializer.SerializeValue(nameof(TagKeyMatchKind), TagKeyMatchKind);
			serializer.SerializeValue(nameof(NewTagKey), NewTagKey);
			serializer.SerializeValue(nameof(ConflictResolution), ConflictResolution);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override void ApplyEffectsCore(AttackState state, TagCollection target)
		{
			foreach (var tag in state.GetTags(TagScope))
			{
				if (MatchKindUtility.IsMatch(tag.Key, TagKey, TagKeyMatchKind, false))
				{
					var newTag = tag.CloneWithKey(NewTagKey);
					target.AddTag(newTag, ConflictResolution);
				}
			}
		}

		protected override bool IsValidCore(List<string> errors)
		{
			throw new System.NotImplementedException();
		}

		private CopyTagsEffect(IDeserializer deserializer)
			: base(deserializer)
		{
			TagKey = deserializer.GetValue<string>(nameof(TagKey));
			TagScope = deserializer.GetValue<TagScope>(nameof(TagScope));
			TagKeyMatchKind = deserializer.GetValue<MatchKind>(nameof(TagKeyMatchKind));
			NewTagKey = deserializer.GetValue<string>(nameof(NewTagKey));
			ConflictResolution = deserializer.GetValue<KeyConflictResolutionKind>(nameof(ConflictResolution));
		}

		static CopyTagsEffect()
		{
			SerializationManager.RegisterSerializable(nameof(CopyTagsEffect), x => new CopyTagsEffect(x));
		}
	}
}
