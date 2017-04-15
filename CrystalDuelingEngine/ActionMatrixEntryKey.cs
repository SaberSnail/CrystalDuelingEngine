using System;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine
{
	public sealed class ActionMatrixEntryKey : IEquatable<ActionMatrixEntryKey>
	{
		public ActionMatrixEntryKey(string attackerActionId, string defenderActionId)
		{
			AttackerActionId = attackerActionId;
			DefenderActionId = defenderActionId;
		}

		public string AttackerActionId { get; }
		public string DefenderActionId { get; }

		public override bool Equals(object that)
		{
			return Equals(that as ActionMatrixEntryKey);
		}

		public bool Equals(ActionMatrixEntryKey that)
		{
			return that != null && AttackerActionId == that.AttackerActionId && DefenderActionId == that.DefenderActionId;
		}

		public static bool operator ==(ActionMatrixEntryKey left, ActionMatrixEntryKey right)
		{
			return ObjectImpl.OperatorEquality(left, right);
		}

		public static bool operator !=(ActionMatrixEntryKey left, ActionMatrixEntryKey right)
		{
			return ObjectImpl.OperatorInequality(left, right);
		}

		public override int GetHashCode()
		{
			return HashCodeUtility.CombineHashCodes(AttackerActionId.GetHashCode(), DefenderActionId.GetHashCode());
		}
	}
}
