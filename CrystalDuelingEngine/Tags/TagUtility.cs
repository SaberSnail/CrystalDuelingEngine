using System.Collections.Generic;
using System.Linq;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Tags
{
	public static class TagUtility
	{
		public static IEnumerable<TagBase> GetTags(this IEnumerable<TagBase> tags, string match, MatchKind matchKind)
		{
			return tags.EmptyIfNull().Where(x => MatchKindUtility.IsMatch(x.Key, match, matchKind, false));
		}
	}
}
