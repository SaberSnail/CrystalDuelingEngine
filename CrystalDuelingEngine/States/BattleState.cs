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
	public sealed class BattleState : IRenderable
	{
		public BattleState(BattleRules battleRules, EntityState entity1, EntityState entity2)
		{
			BattleRules = battleRules;
			Tags = new TagCollection(this);
			m_entity1 = entity1;
			m_entity2 = entity2;
			m_nextActions = new Dictionary<Guid, string>();

			foreach (EntityState entity in AllEntities())
				entity.JoinBattle(this);

			StartBattle();
		}

		public string RenderForLog()
		{
			return "Battle";
		}

		public string RenderForUi()
		{
			return "Battle";
		}

		public BattleRules BattleRules { get; }

		public GameRules GameRules => BattleRules.GameRules;

		public int TurnNumber { get; private set; }

		public bool IsBattleActive { get; private set; }

		public TagCollection Tags { get; }

		public void StartBattle()
		{
			Log.Info("Starting battle.");

			TurnNumber = 0;
			IsBattleActive = true;

			ApplyTransitionEffectsToAllEntities(GameRules.PreBattleEffects);
			ApplyTransitionEffectsToAllEntities(BattleRules.PreBattleEffects);

			foreach (EntityState entity in AllEntities())
				entity.StartBattle(this);
		}

		public void EndBattle()
		{
			Log.Info("Ending battle.");

			foreach (EntityState entity in AllEntities())
				entity.EndBattle(this);

			ApplyTransitionEffectsToAllEntities(BattleRules.PostBattleEffects);
			ApplyTransitionEffectsToAllEntities(GameRules.PostBattleEffects);

			IsBattleActive = false;
		}

		public void StartTurn()
		{
			if (!IsBattleActive)
				throw new InvalidOperationException("Tried to start turn on inactive battle.");

			TurnNumber++;
			Log.Info($"Starting turn {TurnNumber}.");

			ApplyTransitionEffectsToAllEntities(GameRules.PreTurnEffects);
			ApplyTransitionEffectsToAllEntities(BattleRules.PreTurnEffects);

			Tags.RemoveZeroDurationTags();
			Tags.DecrementDuration();

			foreach (EntityState entity in AllEntities())
				entity.StartTurn(this);
		}

		public void EndTurn()
		{
			Log.Info("Ending turn.");

			foreach (EntityState entity in AllEntities())
				entity.EndTurn(this);

			ApplyTransitionEffectsToAllEntities(BattleRules.PostTurnEffects);
			ApplyTransitionEffectsToAllEntities(GameRules.PostTurnEffects);

			m_nextActions.Clear();

			if (AllEntities().Any(x => x.IsEliminated(this)))
				EndBattle();
		}

		public void SetAction(EntityState attacker, string actionKey)
		{
			m_nextActions[attacker.Key] = actionKey;
			Log.Info($"{attacker.RenderForLog()} chose {actionKey}.");
		}

		public Result GetResult(EntityState attacker)
		{
			var defender = AllEntities().FirstOrDefault(x => x != attacker);
			if (attacker == null || defender == null)
				return null;
			return defender.GetResult(m_nextActions[attacker.Key], m_nextActions[defender.Key]);
		}

		public void ResolveActions()
		{
			foreach (var entity in AllEntities())
				ResolveActions(entity);
		}

		private void ResolveActions(EntityState attacker)
		{
			var nextAttackerActionKey = m_nextActions[attacker.Key];
			var attackerAction = attacker.GetActions().First(x => x.Key == nextAttackerActionKey);

			var defender = GetOpponent(attacker);
			var nextDefenderActionKey = m_nextActions[defender.Key];
			var defenderAction = defender.GetActions().First(x => x.Key == nextDefenderActionKey);

			var attackerResult = GetResult(attacker);
			var defenderResult = GetResult(defender);

			Log.Info($"{attacker.RenderForLog()} is {attackerResult.RenderForLog()}");
			var workspace = new TagCollection(defender);
			var attackState = new AttackState();
			attackState.SetTags(TagScope.Battle, Tags);
			attackState.SetTags(TagScope.AttackerAction, attackerAction.Tags);
			attackState.SetTags(TagScope.DefenderAction, defenderAction.Tags);
			attackState.SetTags(TagScope.AttackerResult, attackerResult.Tags);
			attackState.SetTags(TagScope.DefenderResult, defenderResult.Tags);
			attackState.SetTags(TagScope.Attacker, attacker.Tags);
			attackState.SetTags(TagScope.Defender, defender.Tags);
			attackState.SetDynamicTags(TagScope.TemporaryWorkspace, workspace);

			ApplyResultEffects(attackState, attacker, defender, attackerAction, defenderAction, workspace, attackerResult.Effects);
		}

		private void ApplyResultEffects(AttackState state, EntityState attacker, EntityState defender, Action attackerAction, Action defenderAction, TagCollection workspace, ReadOnlyCollection<EffectBase> effects)
		{
			foreach (var effect in GameRules.PreResultEffects)
				ApplyEffectsToEntity(state, attacker, defender, attackerAction, defenderAction, workspace, effect);

			foreach (EffectBase effect in effects)
				ApplyEffectsToEntity(state, attacker, defender, attackerAction, defenderAction, workspace, effect);

			foreach (var effect in GameRules.PostResultEffects)
				ApplyEffectsToEntity(state, attacker, defender, attackerAction, defenderAction, workspace, effect);
		}

		private void ApplyEffectsToEntity(AttackState state, EntityState attacker, EntityState defender, Action attackerAction,
			Action defenderAction, TagCollection workspace, EffectBase effect)
		{
			switch (effect.Target)
			{
			case EffectTarget.Attacker:
			case EffectTarget.AttackerActions:
				attacker.ApplyEffects(state, new[] { effect });
				break;
			case EffectTarget.Defender:
			case EffectTarget.DefenderActions:
				defender.ApplyEffects(state, new[] { effect });
				break;
			case EffectTarget.AttackerSelectedAction:
				effect.ApplyEffects(state, attackerAction.Tags);
				break;
			case EffectTarget.DefenderSelectedAction:
				effect.ApplyEffects(state, defenderAction.Tags);
				break;
			case EffectTarget.TemporaryWorkspace:
				effect.ApplyEffects(state, workspace);
				break;
			}
		}

		private void ApplyTransitionEffectsToAllEntities(ReadOnlyCollection<EffectBase> effects)
		{
			foreach (EntityState entity in AllEntities())
			{
				AttackState state = new AttackState();
				state.SetTags(TagScope.Battle, Tags);
				state.SetTags(TagScope.Attacker, entity.Tags);
				var workspace = new TagCollection(entity);
				state.SetDynamicTags(TagScope.TemporaryWorkspace, workspace);

				entity.ApplyEffects(state, effects);
			}
		}

		private IEnumerable<EntityState> AllEntities()
		{
			yield return m_entity1;
			yield return m_entity2;
		}

		private EntityState GetOpponent(EntityState entity)
		{
			return entity == m_entity1 ? m_entity2 : m_entity1;
		}

		static readonly ILogSource Log = LogManager.CreateLogSource(nameof(BattleState));

		readonly Dictionary<Guid, string> m_nextActions;
		readonly EntityState m_entity1;
		readonly EntityState m_entity2;
	}
}
