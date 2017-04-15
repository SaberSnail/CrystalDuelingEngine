namespace CrystalDuelingEngine.Tags
{
	public static class SystemTagUtility
	{
		public static readonly string MaxTargetsTagKey = CreateSystemTagKey("MaxTargets");
		public static readonly string ActionLimitKey = CreateSystemTagKey("ActionLimit");

		private static string CreateSystemTagKey(string key)
		{
			return "$" + key;
		}
	}
}
