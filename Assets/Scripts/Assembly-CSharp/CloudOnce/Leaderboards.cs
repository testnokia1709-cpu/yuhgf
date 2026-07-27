using CloudOnce.Internal;

namespace CloudOnce
{
	public static class Leaderboards
	{
		private static readonly UnifiedLeaderboard s_fastestTime = new UnifiedLeaderboard("FastestTime", "CgkIw6z7j9ETEAIQFw");

		private static readonly UnifiedLeaderboard s_fewestShapes = new UnifiedLeaderboard("FewestShapes", "CgkIw6z7j9ETEAIQGA");

		public static UnifiedLeaderboard FastestTime
		{
			get
			{
				return s_fastestTime;
			}
		}

		public static UnifiedLeaderboard FewestShapes
		{
			get
			{
				return s_fewestShapes;
			}
		}
	}
}
