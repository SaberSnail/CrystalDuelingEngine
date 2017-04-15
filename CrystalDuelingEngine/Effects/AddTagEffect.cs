using System.Collections.Generic;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine.Effects
{
	public sealed class AddTagEffect : EffectBase
	{
		public AddTagEffect(TagBase tag)
			: this(tag, KeyConflictResolutionKind.Replace, AlwaysCondition.True, EffectTarget.Defender)
		{
		}

		public AddTagEffect(TagBase tag, KeyConflictResolutionKind conflictResolution)
			: this(tag, conflictResolution, AlwaysCondition.True, EffectTarget.Defender)
		{
		}

		public AddTagEffect(TagBase tag, EffectTarget target)
			: this(tag, KeyConflictResolutionKind.Replace, AlwaysCondition.True, target)
		{
		}

		public AddTagEffect(TagBase tag, KeyConflictResolutionKind conflictResolution, ConditionBase condition, EffectTarget target)
			: base(condition, target)
		{
			Tag = tag;
			ConflictResolution = conflictResolution;
		}

		public override string SerializationName => nameof(AddTagEffect);

		public TagBase Tag { get; }

		public KeyConflictResolutionKind ConflictResolution { get; }

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Tag), Tag);
			serializer.SerializeValue(nameof(ConflictResolution), ConflictResolution);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override void ApplyEffectsCore(AttackState state, TagCollection target)
		{
			target.AddTag(Tag, ConflictResolution);
		}

		protected override bool IsValidCore(List<string> errors)
		{
			throw new System.NotImplementedException();
		}

		private AddTagEffect(IDeserializer deserializer)
			: base(deserializer)
		{
			Tag = deserializer.GetValue<TagBase>(nameof(Tag));
			ConflictResolution = deserializer.GetValue<KeyConflictResolutionKind>(nameof(ConflictResolution));
		}

		static AddTagEffect()
		{
			SerializationManager.RegisterSerializable(nameof(AddTagEffect), x => new AddTagEffect(x));
		}
	}
}
