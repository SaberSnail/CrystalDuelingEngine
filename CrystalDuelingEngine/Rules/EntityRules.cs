using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrystalDuelingEngine.Conditions;
using CrystalDuelingEngine.Effects;
using CrystalDuelingEngine.Serialization;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Rules
{
	public sealed class EntityRules : IRenderable, ISerializable, ILocalizationSource
	{
		public EntityRules(string name, StringLookup lookup)
		{
			ActionMatrix = new Dictionary<ActionMatrixEntryKey, ActionMatrixEntry>();
			PreBattleEffects = new List<EffectBase>().AsReadOnly();
			PostBattleEffects = new List<EffectBase>().AsReadOnly();
			PreTurnEffects = new List<EffectBase>().AsReadOnly();
			PostTurnEffects = new List<EffectBase>().AsReadOnly();
			EliminationCondition = AlwaysCondition.False;
			Actions = new List<Action>().AsReadOnly();
			Results = new List<Result>().AsReadOnly();
			Name = name;
			m_lookup = lookup;
		}

		public string SerializationName => nameof(EntityRules);

		public string Name { get; }

		/// <summary>
		/// Valid TagScopes are Attacker (this entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PreBattleEffects { get; set; }

		/// <summary>
		/// Valid TagScopes are Attacker (this entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PostBattleEffects { get; set; }

		/// <summary>
		/// Valid TagScopes are Attacker (this entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PreTurnEffects { get; set; }

		/// <summary>
		/// Valid TagScopes are Attacker (this entity) and Battle.
		/// Valid EffectTargets are all Attacker targets except AttackerSelectedAction.
		/// </summary>
		public ReadOnlyCollection<EffectBase> PostTurnEffects { get; set; }

		/// <summary>
		/// Valid TagScopes are Attacker (this entity) and Battle.
		/// </summary>
		public ConditionBase EliminationCondition { get; set; }

		public ReadOnlyCollection<Action> Actions { get; set; }
		public ReadOnlyCollection<Result> Results { get; set; }

		private Dictionary<ActionMatrixEntryKey, ActionMatrixEntry> ActionMatrix { get; }

		public void AddToActionMatrix(string attackerActionId, IEnumerable<Tuple<string, string>> defenderActionIdToResults)
		{
			foreach (Tuple<string, string> data in defenderActionIdToResults)
			{
				ActionMatrixEntry entry = new ActionMatrixEntry(attackerActionId, data.Item1, data.Item2);
				ActionMatrix[entry.Key] = entry;
			}
		}

		public Result GetResult(string attackerActionKey, string defenderActionKey)
		{
			ActionMatrixEntry entry;
			if (ActionMatrix.TryGetValue(new ActionMatrixEntryKey(attackerActionKey, defenderActionKey), out entry))
			{
				var result = Results.FirstOrDefault(x => x.Key == entry.ResultId);
				if (result == null)
					throw new InvalidRulesException($"Entity '{RenderForLog()}' is missing result '{entry.ResultId}'.");
				return result;
			}

			throw new InvalidRulesException($"Entity '{RenderForLog()}' is missing entries in the action matrix. Attacker='{attackerActionKey}', Defender='{defenderActionKey}'");
		}

		public string Lookup(string key)
		{
			return m_lookup.Lookup(key);
		}

		public string RenderForLog()
		{
			return Lookup(Name);
		}

		public string RenderForUi()
		{
			return Lookup(Name);
		}

		public void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(Name), Name);
			serializer.SerializeValue("Lookup", m_lookup);
			serializer.SerializeValue(nameof(PreBattleEffects), PreBattleEffects);
			serializer.SerializeValue(nameof(PostBattleEffects), PostBattleEffects);
			serializer.SerializeValue(nameof(PreTurnEffects), PreTurnEffects);
			serializer.SerializeValue(nameof(PostTurnEffects), PostTurnEffects);
			serializer.SerializeValue(nameof(EliminationCondition), EliminationCondition);
			serializer.SerializeValue(nameof(Actions), Actions);
			serializer.SerializeValue(nameof(Results), Results);
			serializer.SerializeValue(nameof(ActionMatrix), ActionMatrix.Values);

			serializer.EndObject();
		}

		private EntityRules(IDeserializer deserializer)
		{
			ActionMatrix = new Dictionary<ActionMatrixEntryKey, ActionMatrixEntry>();
			Name = deserializer.GetValue<string>(nameof(Name));
			m_lookup = deserializer.GetValue<StringLookup>("Lookup");
			PreBattleEffects = deserializer.GetValues<EffectBase>(nameof(PreBattleEffects)).ToList().AsReadOnly();
			PostBattleEffects = deserializer.GetValues<EffectBase>(nameof(PostBattleEffects)).EmptyIfNull().ToList().AsReadOnly();
			PreTurnEffects = deserializer.GetValues<EffectBase>(nameof(PreTurnEffects)).EmptyIfNull().ToList().AsReadOnly();
			PostTurnEffects = deserializer.GetValues<EffectBase>(nameof(PostTurnEffects)).EmptyIfNull().ToList().AsReadOnly();
			EliminationCondition = deserializer.GetValue<ConditionBase>(nameof(EliminationCondition));
			Actions = deserializer.GetValues<Action>(nameof(Actions)).EmptyIfNull().ToList().AsReadOnly();
			Results = deserializer.GetValues<Result>(nameof(Results)).EmptyIfNull().ToList().AsReadOnly();
			ActionMatrix = deserializer.GetValues<ActionMatrixEntry>(nameof(ActionMatrix)).EmptyIfNull().ToDictionary(x => x.Key, x => x);
		}

		static EntityRules()
		{
			SerializationManager.RegisterSerializable(nameof(EntityRules), x => new EntityRules(x));
		}

		readonly StringLookup m_lookup;
	}
}
