using System.Collections.Generic;
using System.Linq;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using CrystalDuelingEngine.Tags;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Conditions
{
	public abstract class TagMatchCondition : ConditionBase
	{
		public override string SerializationName => nameof(TagMatchCondition);

		public TagScope MatchScopes { get; }

		public string MatchKey { get; }

		public MatchKind KeyMatchKind { get; }

		public override bool IsTrue(AttackState state)
		{
			return state.GetTags(MatchScopes)
				.Where(x => MatchKindUtility.IsMatch(x.Key, MatchKey, KeyMatchKind, false))
				.Any(IsTagValueMatch);
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.SerializeValue(nameof(MatchScopes), MatchScopes);
			serializer.SerializeValue(nameof(MatchKey), MatchKey);
			serializer.SerializeValue(nameof(KeyMatchKind), KeyMatchKind);
			base.Serialize(serializer);
		}

		protected TagMatchCondition(TagScope matchScopes, string matchKey, MatchKind keyMatchKind)
		{
			MatchScopes = matchScopes;
			MatchKey = matchKey;
			KeyMatchKind = keyMatchKind;
		}

		protected TagMatchCondition(IDeserializer deserializer)
			: base(deserializer)
		{
			MatchScopes = deserializer.GetValue<TagScope>(nameof(MatchScopes));
			MatchKey = deserializer.GetValue<string>(nameof(MatchKey));
			KeyMatchKind = deserializer.GetValue<MatchKind>(nameof(KeyMatchKind));
		}

		protected abstract bool IsTagValueMatch(TagBase tag);

		protected override bool IsValidCore(List<string> errors)
		{
			if (MatchScopes == TagScope.None)
			{
				errors.Add(OurResources.InvalidConditionMissingMatchScope.FormatCurrentUiCulture(GetType().Name));
				return false;
			}
			if (MatchKey == null)
			{
				errors.Add(OurResources.InvalidConditionMissingMatchKey.FormatCurrentUiCulture(GetType().Name));
				return false;
			}

			return true;
		}
	}
}
