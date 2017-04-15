using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Effects;
using CrystalDuelingEngine.Serialization;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Rules
{
	public sealed class BattleRules : ISerializable
	{
		public BattleRules()
		{
			PreBattleEffects = new List<EffectBase>().AsReadOnly();
			PostBattleEffects = new List<EffectBase>().AsReadOnly();
			PreTurnEffects = new List<EffectBase>().AsReadOnly();
			PostTurnEffects = new List<EffectBase>().AsReadOnly();
			EliminationCondition = AlwaysCondition.False;
		}

		public string SerializationName => nameof(BattleRules);

		public GameRules GameRules { get; set; }

		/// <summary>
		/// Effects are applied to all entities.
		/// Valid TagScopes are Attacker (the entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PreBattleEffects { get; set; }

		/// <summary>
		/// Effects are applied to all entities.
		/// Valid TagScopes are Attacker (the entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PostBattleEffects { get; set; }

		/// <summary>
		/// Effects are applied to all entities.
		/// Valid TagScopes are Attacker (the entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PreTurnEffects { get; set; }

		/// <summary>
		/// Effects are applied to all entities.
		/// Valid TagScopes are Attacker (the entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PostTurnEffects { get; set; }

		/// <summary>
		/// Valid TagScopes are Attacker (this entity) and Battle.
		/// </summary>
		public ConditionBase EliminationCondition { get; set; }

		public void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(PreBattleEffects), PreBattleEffects);
			serializer.SerializeValue(nameof(PostBattleEffects), PostBattleEffects);
			serializer.SerializeValue(nameof(PreTurnEffects), PreTurnEffects);
			serializer.SerializeValue(nameof(PostTurnEffects), PostTurnEffects);
			serializer.SerializeValue(nameof(EliminationCondition), EliminationCondition);

			serializer.EndObject();
		}

		private BattleRules(IDeserializer deserializer)
		{
			PreBattleEffects = deserializer.GetValues<EffectBase>(nameof(PreBattleEffects)).ToList().AsReadOnly();
			PostBattleEffects = deserializer.GetValues<EffectBase>(nameof(PostBattleEffects)).EmptyIfNull().ToList().AsReadOnly();
			PreTurnEffects = deserializer.GetValues<EffectBase>(nameof(PreTurnEffects)).EmptyIfNull().ToList().AsReadOnly();
			PostTurnEffects = deserializer.GetValues<EffectBase>(nameof(PostTurnEffects)).EmptyIfNull().ToList().AsReadOnly();
			EliminationCondition = deserializer.GetValue<ConditionBase>(nameof(EliminationCondition));
		}

		static BattleRules()
		{
			SerializationManager.RegisterSerializable(nameof(BattleRules), x => new BattleRules(x));
		}
	}
}
