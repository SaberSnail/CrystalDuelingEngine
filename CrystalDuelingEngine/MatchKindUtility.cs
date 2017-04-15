using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CrystalDuelingEngine
{
	public static class MatchKindUtility
	{
		public static bool IsMatch(string test, string match, MatchKind matchKind, bool isCaseSensitive)
		{
			var compareInfo = CultureInfo.CurrentCulture.CompareInfo;
			var options = isCaseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;
			switch (matchKind)
			{
			case MatchKind.Exact:
				return compareInfo.Compare(test, match, options) == 0;
			case MatchKind.Contains:
				return compareInfo.IndexOf(test, match, options) != -1;
			case MatchKind.Prefix:
				return compareInfo.IsPrefix(test, match, options);
			case MatchKind.Suffix:
				return compareInfo.IsSuffix(test, match, options);
			case MatchKind.Regex:
			{
				var regex = new Regex(match, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
				return regex.IsMatch(test);
			}
			default:
				throw new NotImplementedException($"Unhandled MatchKind: {matchKind}");
			}
		}
	}
}
