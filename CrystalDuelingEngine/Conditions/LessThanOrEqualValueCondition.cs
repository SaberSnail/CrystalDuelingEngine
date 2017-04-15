using CrystalDuelingEngine.Serialization;

namespace CrystalDuelingEngine.Conditions
{
	public sealed class LessThanOrEqualValueCondition : ValueCompareCondition
	{
		public LessThanOrEqualValueCondition(TagScope matchScopes, string matchKey, int matchValue)
			: base(matchScopes, matchKey, MatchKind.Exact, matchValue)
		{
		}

		public LessThanOrEqualValueCondition(TagScope matchScopes, string matchKey, MatchKind keyMatchKind, int matchValue)
			: base(matchScopes, matchKey, keyMatchKind, matchValue)
		{
		}

		public LessThanOrEqualValueCondition(TagScope matchScopes, string matchKey, string matchValue)
			: base(matchScopes, matchKey, MatchKind.Exact, matchValue)
		{
		}

		public LessThanOrEqualValueCondition(TagScope matchScopes, string matchKey, MatchKind keyMatchKind, string matchValue)
			: base(matchScopes, matchKey, keyMatchKind, matchValue)
		{
		}

		public override string SerializationName => nameof(LessThanOrEqualValueCondition);

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override string RenderOperatorForLog()
		{
			return "<=";
		}

		protected override string RenderOperatorForUi()
		{
			return "<=";
		}

		protected override bool CompareOperationCore(int compareValue)
		{
			return compareValue <= 0;
		}

		private LessThanOrEqualValueCondition(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static LessThanOrEqualValueCondition()
		{
			SerializationManager.RegisterSerializable(nameof(LessThanOrEqualValueCondition), x => new LessThanOrEqualValueCondition(x));
		}
	}
}
