using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrystalDuelingEngine.Effects;
using CrystalDuelingEngine.Rules;
using CrystalDuelingEngine.Tags;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;

namespace CrystalDuelingEngine.States
{
	public sealed class EntityState : IRenderable, ILocalizationSource
	{
		public EntityState(EntityRules entityRules)
		{
			m_entityRules = entityRules;
			Tags = new TagCollection(this);
			m_actions = entityRules.Actions.Select(x => x.Clone()).ToList().AsReadOnly();
			Key = Guid.NewGuid();
		}

		public string Name { get; set; }

		public Guid Key { get; }

		public string EntityRulesName => m_entityRules.Lookup(m_entityRules.Name);

		public string RenderForLog()
		{
			return Name;
		}

		public string RenderForUi()
		{
			return Name;
		}

		public string Lookup(string key)
		{
			return m_entityRules.Lookup(key);
		}

		public TagCollection Tags { get; }

		public void JoinBattle(BattleState battleState)
		{
			Log.Info($"{RenderForLog()} is joining a battle.");
		}

		public void StartBattle(BattleState battleState)
		{
			Log.Info($"Starting battle for {RenderForLog()}.");
	
			ApplyTransitionEffects(battleState, m_entityRules.PreBattleEffects);
		}

		public void EndBattle(BattleState battleState)
		{
			Log.Info($"Ending battle for {RenderForLog()}.");

			ApplyTransitionEffects(battleState, m_entityRules.PostBattleEffects);
		}

		public void StartTurn(BattleState battleState)
		{
			Log.Info($"Starting turn for {RenderForLog()}.");

			ApplyTransitionEffects(battleState, m_entityRules.PreTurnEffects);

			Tags.RemoveZeroDurationTags();
			Tags.DecrementDuration();

			foreach (var action in m_actions)
			{
				action.Tags.RemoveZeroDurationTags();
				action.Tags.DecrementDuration();
			}
		}

		public void EndTurn(BattleState battleState)
		{
			Log.Info($"Ending turn for {RenderForLog()}.");

			ApplyTransitionEffects(battleState, m_entityRules.PostTurnEffects);
		}

		public bool IsEliminated(BattleState battleState)
		{
			AttackState attackState = new AttackState();
			attackState.SetTags(TagScope.Attacker, Tags);
			attackState.SetTags(TagScope.Battle, battleState.Tags);
			return battleState.GameRules.EliminationCondition.IsTrue(attackState) ||
				battleState.BattleRules.EliminationCondition.IsTrue(attackState) ||
				m_entityRules.EliminationCondition.IsTrue(attackState);
		}

		public ReadOnlyCollection<Action> GetActions()
		{
			return m_actions;
		}

		public ReadOnlyCollection<Action> GetAvailableActions(BattleState battleState)
		{
			AttackState attackState = new AttackState();
			attackState.SetTags(TagScope.Attacker, Tags);
			attackState.SetTags(TagScope.Battle, battleState.Tags);
			var workspace = new TagCollection(this);
			attackState.SetDynamicTags(TagScope.TemporaryWorkspace, workspace);

			ReadOnlyCollection<ConditionTagBase> entityLimitTags = Tags.Tags
				.Concat(battleState.Tags.Tags)
				.OfType<ConditionTagBase>()
				.Where(x => x.Key == SystemTagUtility.ActionLimitKey)
				.ToList()
				.AsReadOnly();
			return m_actions.Where(action =>
			{
				attackState.SetTags(TagScope.CurrentAction, action.Tags);

				if (entityLimitTags.Count != 0 && entityLimitTags.Any(x => !x.IsTrue(attackState)))
					return false;

				ReadOnlyCollection<ConditionTagBase> actionLimitTags = action.Tags.OfType<ConditionTagBase>().Where(x => x.Key == SystemTagUtility.ActionLimitKey).ToList().AsReadOnly();
				return actionLimitTags.Count == 0 || actionLimitTags.All(x => x.IsTrue(attackState));
			}).ToList().AsReadOnly();
		}

		public Result GetResult(string attackerActionKey, string defenderActionKey)
		{
			return m_entityRules.GetResult(attackerActionKey, defenderActionKey);
		}

		public void ApplyEffects(AttackState state, IEnumerable<EffectBase> effects)
		{
			foreach (EffectBase effect in effects.EmptyIfNull())
			{
				if (effect.Target == EffectTarget.Attacker || effect.Target == EffectTarget.Defender)
				{
					effect.ApplyEffects(state, Tags);
				}
				else if (effect.Target == EffectTarget.AttackerActions || effect.Target == EffectTarget.DefenderActions)
				{
					foreach (var action in m_actions)
					{
						state.SetTags(TagScope.CurrentAction, action.Tags);
						effect.ApplyEffects(state, action.Tags);
					}
					state.SetTags(TagScope.CurrentAction, null);
				}
				else
				{
					throw new InvalidRulesException($"Invalid target: {effect.Target}");
				}
			}
		}

		private void ApplyTransitionEffects(BattleState battleState, IEnumerable<EffectBase> effects)
		{
			AttackState state = new AttackState();
			state.SetTags(TagScope.Battle, battleState.Tags);
			state.SetTags(TagScope.Attacker, Tags);
			var workspace = new TagCollection(this);
			state.SetDynamicTags(TagScope.TemporaryWorkspace, workspace);

			ApplyEffects(state, effects);
		}

		private static readonly ILogSource Log = LogManager.CreateLogSource(nameof(EntityState));

		readonly EntityRules m_entityRules;
		readonly ReadOnlyCollection<Action> m_actions;
	}
}
