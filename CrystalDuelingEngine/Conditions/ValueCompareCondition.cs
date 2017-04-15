using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine.Conditions
{
	public abstract class ValueCompareCondition : TagMatchCondition
	{
		public override string SerializationName => nameof(ValueCompareCondition);

		public int? CompareIntValue { get; }

		public string CompareStringValue { get; }

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue(nameof(CompareIntValue), CompareIntValue);
			serializer.SerializeValue(nameof(CompareStringValue), CompareStringValue);
			base.Serialize(serializer);

			serializer.EndObject();
		}

		public override string RenderForLog()
		{
			return $"{MatchKey} ({MatchScopes}) {RenderOperatorForLog()} {CompareStringValue ?? CompareIntValue.ToString()}";
		}

		public override string RenderForUi()
		{
			return $"{MatchKey} ({MatchScopes}) {RenderOperatorForUi()} {CompareStringValue ?? CompareIntValue.ToString()}";
		}

		protected ValueCompareCondition(TagScope matchScopes, string matchKey, MatchKind keyMatchKind, int matchValue)
			: base(matchScopes, matchKey, keyMatchKind)
		{
			CompareIntValue = matchValue;
		}

		protected ValueCompareCondition(TagScope matchScopes, string matchKey, MatchKind keyMatchKind, string matchValue)
			: base(matchScopes, matchKey, keyMatchKind)
		{
			CompareStringValue = matchValue;
		}

		protected ValueCompareCondition(IDeserializer deserializer)
			: base(deserializer)
		{
			CompareIntValue = deserializer.GetValue<int?>(nameof(CompareIntValue));
			CompareStringValue = deserializer.GetValue<string>(nameof(CompareStringValue));
		}

		protected override bool IsTagValueMatch(TagBase tag)
		{
			ValueTagBase compareTag = tag as ValueTagBase;
			if (compareTag == null)
				return false;

			ValueTagBase valueTag = CompareIntValue.HasValue
				? (ValueTagBase) new IntValueTag("temp", CompareIntValue)
				: (ValueTagBase) new StringValueTag("temp", CompareStringValue);
			return CompareOperationCore(compareTag.CompareTo(valueTag));
		}

		protected abstract bool CompareOperationCore(int compareValue);

		protected abstract string RenderOperatorForLog();

		protected abstract string RenderOperatorForUi();
	}
}
