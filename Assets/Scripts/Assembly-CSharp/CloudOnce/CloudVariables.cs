using CloudOnce.CloudPrefs;

namespace CloudOnce
{
	public static class CloudVariables
	{
		private static readonly CloudString s_levelsSolved = new CloudString("LevelsSolved", PersistenceType.Latest, string.Empty);

		private static readonly CloudString s_levelsLocked = new CloudString("LevelsLocked", PersistenceType.Latest, string.Empty);

		private static readonly CloudString s_levelsMinShapeCount = new CloudString("LevelsMinShapeCount", PersistenceType.Latest, string.Empty);

		private static readonly CloudString s_levelsMinTime = new CloudString("LevelsMinTime", PersistenceType.Latest, string.Empty);

		private static readonly CloudInt s_shapeCount = new CloudInt("ShapeCount", PersistenceType.Highest);

		private static readonly CloudString s_freeItems = new CloudString("FreeItems", PersistenceType.Latest, string.Empty);

		private static readonly CloudInt s_coinCount = new CloudInt("CoinCount", PersistenceType.Latest);

		private static readonly CloudInt s_gemCount = new CloudInt("GemCount", PersistenceType.Latest);

		public static string LevelsSolved
		{
			get
			{
				return s_levelsSolved.Value;
			}
			set
			{
				s_levelsSolved.Value = value;
			}
		}

		public static string LevelsLocked
		{
			get
			{
				return s_levelsLocked.Value;
			}
			set
			{
				s_levelsLocked.Value = value;
			}
		}

		public static string LevelsMinShapeCount
		{
			get
			{
				return s_levelsMinShapeCount.Value;
			}
			set
			{
				s_levelsMinShapeCount.Value = value;
			}
		}

		public static string LevelsMinTime
		{
			get
			{
				return s_levelsMinTime.Value;
			}
			set
			{
				s_levelsMinTime.Value = value;
			}
		}

		public static int ShapeCount
		{
			get
			{
				return s_shapeCount.Value;
			}
			set
			{
				s_shapeCount.Value = value;
			}
		}

		public static string FreeItems
		{
			get
			{
				return s_freeItems.Value;
			}
			set
			{
				s_freeItems.Value = value;
			}
		}

		public static int CoinCount
		{
			get
			{
				return s_coinCount.Value;
			}
			set
			{
				s_coinCount.Value = value;
			}
		}

		public static int GemCount
		{
			get
			{
				return s_gemCount.Value;
			}
			set
			{
				s_gemCount.Value = value;
			}
		}
	}
}
