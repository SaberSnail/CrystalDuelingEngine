using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.Tags;

namespace CrystalDuelingEngine.Conditions
{
	public sealed class HasTagCondition : TagMatchCondition
	{
		public HasTagCondition(TagScope matchScopes, string matchKey)
			: base(matchScopes, matchKey, MatchKind.Exact)
		{
		}

		public HasTagCondition(TagScope matchScopes, string matchKey, MatchKind keyMatchKind)
			: base(matchScopes, matchKey, keyMatchKind)
		{
		}

		public override string SerializationName => nameof(HasTagCondition);

		public override string RenderForLog()
		{
			return $"HAS TAG {MatchKey} ({MatchScopes})";
		}

		public override string RenderForUi()
		{
			return $"HAS TAG {MatchKey} ({MatchScopes})";
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			base.Serialize(serializer);

			serializer.EndObject();
		}

		protected override bool IsTagValueMatch(TagBase tag)
		{
			return true;
		}

		private HasTagCondition(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		static HasTagCondition()
		{
			SerializationManager.RegisterSerializable(nameof(HasTagCondition), x => new HasTagCondition(x));
		}
	}
}
