using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;

	public static Vector2 ThumbnailSize = new Vector2(120f, 80f);

	public static Dictionary<string, Texture2D> ThumbnailCache = new Dictionary<string, Texture2D>();

	public static int PackLevelCount = 20;

	private static int s_levelGroupCount = 5;

	private static int s_levelsSolvedToUnlockGroup = 4;

	private AssetBundle m_bundleLevels;

	private AssetBundleCreateRequest bundleLoadRequest;

	private static List<LevelPack> s_levelPacks = new List<LevelPack>
	{
		new LevelPack
		{
			Item = PurchasableItem.PACK_1,
			FirstLevel = 1,
			LastLevel = 20,
			StarsForGift = 0,
			Available = true,
			Free = true,
			CheatUnlocks = true,
			Name = "Puzzle Pack 1"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_2,
			FirstLevel = 21,
			LastLevel = 40,
			StarsForGift = 30,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 2"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_3,
			FirstLevel = 41,
			LastLevel = 60,
			StarsForGift = 60,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 3"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_4,
			FirstLevel = 61,
			LastLevel = 80,
			StarsForGift = 105,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 4"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_5,
			FirstLevel = 81,
			LastLevel = 100,
			StarsForGift = 150,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 5"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_6,
			FirstLevel = 101,
			LastLevel = 120,
			StarsForGift = 195,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 6"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_7,
			FirstLevel = 121,
			LastLevel = 140,
			StarsForGift = 255,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 7"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_8,
			FirstLevel = 141,
			LastLevel = 160,
			StarsForGift = 315,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 8"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_9,
			FirstLevel = 161,
			LastLevel = 180,
			StarsForGift = 375,
			Available = true,
			Free = false,
			CheatUnlocks = true,
			Name = "Puzzle Pack 9"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_10,
			FirstLevel = 181,
			LastLevel = 200,
			StarsForGift = 465,
			Available = true,
			Free = false,
			CheatUnlocks = false,
			Name = "Puzzle Pack 10"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_11,
			FirstLevel = 201,
			LastLevel = 220,
			StarsForGift = 555,
			Available = true,
			Free = false,
			CheatUnlocks = false,
			Name = "Puzzle Pack 11"
		},
		new LevelPack
		{
			Item = PurchasableItem.PACK_12,
			FirstLevel = 221,
			LastLevel = 240,
			StarsForGift = 650,
			Available = true,
			Free = false,
			CheatUnlocks = false,
			Name = "Puzzle Pack 12"
		}
	};

	private static List<LevelPack> s_gamePacks = new List<LevelPack>
	{
		new LevelPack
		{
			Item = PurchasableItem.FULL_GAME_100,
			FirstLevel = 1,
			LastLevel = 240,
			StarsForGift = 999,
			Available = true,
			Free = false
		},
		new LevelPack
		{
			Item = PurchasableItem.FULL_GAME_75,
			FirstLevel = 1,
			LastLevel = 240,
			StarsForGift = 999,
			Available = true,
			Free = false
		},
		new LevelPack
		{
			Item = PurchasableItem.FULL_GAME_50,
			FirstLevel = 1,
			LastLevel = 240,
			StarsForGift = 999,
			Available = true,
			Free = false
		},
		new LevelPack
		{
			Item = PurchasableItem.FULL_GAME_25,
			FirstLevel = 1,
			LastLevel = 240,
			StarsForGift = 999,
			Available = true,
			Free = false
		}
	};

	private static Dictionary<int, LevelInfo> s_levelInfo = new Dictionary<int, LevelInfo>
	{
		{
			1,
			new LevelInfo
			{
				File = "L0001",
				Key = "A_01",
				Version = 1,
				Name = "Live_A_01",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_DRAW_A_SHAPE,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			2,
			new LevelInfo
			{
				File = "L0002",
				Key = "A_02",
				Version = 1,
				Name = "Live_A_02",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_LEFTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			3,
			new LevelInfo
			{
				File = "L0003",
				Key = "A_03",
				Version = 1,
				Name = "Live_A_03",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_003,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			4,
			new LevelInfo
			{
				File = "L0004",
				Key = "A_04",
				Version = 1,
				Name = "Live_A_04",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_004,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			5,
			new LevelInfo
			{
				File = "L0007",
				Key = "A_07",
				Version = 1,
				Name = "Live_A_07",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_007,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			6,
			new LevelInfo
			{
				File = "L0006",
				Key = "A_06",
				Version = 2,
				Name = "Live_A_06",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_PLACE_OBJECT_IN_GLASS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			7,
			new LevelInfo
			{
				File = "L0008",
				Key = "A_08",
				Version = 1,
				Name = "Live_A_08",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_008,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			8,
			new LevelInfo
			{
				File = "L0013",
				Key = "A_13",
				Version = 1,
				Name = "Live_A_13",
				ShapeGoal = 2,
				TimeGoal = 7f,
				Hint = StringId.S_013,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			9,
			new LevelInfo
			{
				File = "L0005",
				Key = "A_05",
				Version = 1,
				Name = "Live_A_05",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_005,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			10,
			new LevelInfo
			{
				File = "L0012",
				Key = "A_12",
				Version = 1,
				Name = "Live_A_12",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_012,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			11,
			new LevelInfo
			{
				File = "L0091",
				Key = "A_91",
				Version = 2,
				Name = "Live_A_91",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			12,
			new LevelInfo
			{
				File = "L0093",
				Key = "A_93",
				Version = 2,
				Name = "Live_A_93",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_MAGNET_SEPARATE,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			13,
			new LevelInfo
			{
				File = "L0009",
				Key = "A_09",
				Version = 1,
				Name = "Live_A_09",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_009,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			14,
			new LevelInfo
			{
				File = "L0011",
				Key = "A_11",
				Version = 1,
				Name = "Live_A_11",
				ShapeGoal = 2,
				TimeGoal = 7f,
				Hint = StringId.S_011,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			15,
			new LevelInfo
			{
				File = "L0010",
				Key = "A_10",
				Version = 1,
				Name = "Live_A_10",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			16,
			new LevelInfo
			{
				File = "L0101",
				Key = "A_90",
				Version = 1,
				Name = "Live_A_90",
				ShapeGoal = 2,
				TimeGoal = 16f,
				Hint = StringId.S_028,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			17,
			new LevelInfo
			{
				File = "L0019",
				Key = "A_20",
				Version = 1,
				Name = "Live_A_20",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_020,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			18,
			new LevelInfo
			{
				File = "L0094",
				Key = "A_94",
				Version = 2,
				Name = "Live_A_94",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_MAGNET_SEPARATE,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			19,
			new LevelInfo
			{
				File = "L0092",
				Key = "A_92",
				Version = 2,
				Name = "Live_A_92",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			20,
			new LevelInfo
			{
				File = "L0014",
				Key = "A_14",
				Version = 1,
				Name = "Live_A_14",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_014,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			21,
			new LevelInfo
			{
				File = "L0027",
				Key = "A_29",
				Version = 1,
				Name = "Live_A_29",
				ShapeGoal = 1,
				TimeGoal = 15f,
				Hint = StringId.S_029,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			22,
			new LevelInfo
			{
				File = "L0016",
				Key = "A_16",
				Version = 1,
				Name = "Live_A_16",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_016,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			23,
			new LevelInfo
			{
				File = "L0018",
				Key = "A_19",
				Version = 1,
				Name = "Live_A_19",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			24,
			new LevelInfo
			{
				File = "L0017",
				Key = "A_18",
				Version = 1,
				Name = "Live_A_18",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_018,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			25,
			new LevelInfo
			{
				File = "L0071",
				Key = "A_17",
				Version = 1,
				Name = "Live_A_17",
				ShapeGoal = 1,
				TimeGoal = 9f,
				Hint = StringId.S_017,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			26,
			new LevelInfo
			{
				File = "L0015",
				Key = "A_15",
				Version = 1,
				Name = "Live_A_15",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_CLEAR_OBJECTS_FROM_PLATFORM,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			27,
			new LevelInfo
			{
				File = "L0100",
				Key = "A_100",
				Version = 1,
				Name = "Live_A_100",
				ShapeGoal = 3,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			28,
			new LevelInfo
			{
				File = "L0097",
				Key = "A_97",
				Version = 1,
				Name = "Live_A_97",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			29,
			new LevelInfo
			{
				File = "L0095",
				Key = "A_95",
				Version = 2,
				Name = "Live_A_95",
				ShapeGoal = 3,
				TimeGoal = 13f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			30,
			new LevelInfo
			{
				File = "L0096",
				Key = "A_96",
				Version = 1,
				Name = "Live_A_96",
				ShapeGoal = 2,
				TimeGoal = 15f,
				Hint = StringId.S_031,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			31,
			new LevelInfo
			{
				File = "L0099",
				Key = "A_99",
				Version = 1,
				Name = "Live_A_99",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALLS_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			32,
			new LevelInfo
			{
				File = "L0098",
				Key = "A_98",
				Version = 1,
				Name = "Live_A_98",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALLS_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			33,
			new LevelInfo
			{
				File = "L0026",
				Key = "A_28",
				Version = 1,
				Name = "Live_A_28",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_028,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			34,
			new LevelInfo
			{
				File = "L0020",
				Key = "A_22",
				Version = 1,
				Name = "Live_A_22",
				ShapeGoal = 1,
				TimeGoal = 14f,
				Hint = StringId.S_022,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			35,
			new LevelInfo
			{
				File = "L0033",
				Key = "A_35",
				Version = 1,
				Name = "Live_A_35",
				ShapeGoal = 10,
				TimeGoal = 45f,
				Hint = StringId.S_GET_BALL_OUT_OF_SHAPE,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			36,
			new LevelInfo
			{
				File = "L0074",
				Key = "A_38",
				Version = 1,
				Name = "Live_A_38",
				ShapeGoal = 2,
				TimeGoal = 5f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			37,
			new LevelInfo
			{
				File = "L0072",
				Key = "A_21",
				Version = 1,
				Name = "Live_A_21",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			38,
			new LevelInfo
			{
				File = "L0022",
				Key = "A_24",
				Version = 1,
				Name = "Live_A_24",
				ShapeGoal = 5,
				TimeGoal = 20f,
				Hint = StringId.S_024,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			39,
			new LevelInfo
			{
				File = "L0023",
				Key = "A_25",
				Version = 1,
				Name = "Live_A_25",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_025,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			40,
			new LevelInfo
			{
				File = "L0029",
				Key = "A_31",
				Version = 1,
				Name = "Live_A_31",
				ShapeGoal = 2,
				TimeGoal = 14f,
				Hint = StringId.S_031,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			41,
			new LevelInfo
			{
				File = "L0044",
				Key = "A_49",
				Version = 1,
				Name = "Live_A_49",
				ShapeGoal = 1,
				TimeGoal = 4f,
				Hint = StringId.S_049,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			42,
			new LevelInfo
			{
				File = "L0043",
				Key = "A_48",
				Version = 1,
				Name = "Live_A_48",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			43,
			new LevelInfo
			{
				File = "L0039",
				Key = "A_43",
				Version = 1,
				Name = "Live_A_43",
				ShapeGoal = 1,
				TimeGoal = 15f,
				Hint = StringId.S_GET_ORANGEBALL_OUT_OF_BOX,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			44,
			new LevelInfo
			{
				File = "L0031",
				Key = "A_33",
				Version = 1,
				Name = "Live_A_33",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_033,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			45,
			new LevelInfo
			{
				File = "L0040",
				Key = "A_44",
				Version = 1,
				Name = "Live_A_44",
				ShapeGoal = 4,
				TimeGoal = 15f,
				Hint = StringId.S_044,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			46,
			new LevelInfo
			{
				File = "L0075",
				Key = "A_47",
				Version = 1,
				Name = "Live_A_47",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_047,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			47,
			new LevelInfo
			{
				File = "L0035",
				Key = "A_39",
				Version = 1,
				Name = "Live_A_39",
				ShapeGoal = 2,
				TimeGoal = 6f,
				Hint = StringId.S_039,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			48,
			new LevelInfo
			{
				File = "L0041",
				Key = "A_45",
				Version = 1,
				Name = "Live_A_45",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_045,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			49,
			new LevelInfo
			{
				File = "L0038",
				Key = "A_42",
				Version = 1,
				Name = "Live_A_42",
				ShapeGoal = 2,
				TimeGoal = 6f,
				Hint = StringId.S_042,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			50,
			new LevelInfo
			{
				File = "L0025",
				Key = "A_27",
				Version = 1,
				Name = "Live_A_27",
				ShapeGoal = 3,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			51,
			new LevelInfo
			{
				File = "L0045",
				Key = "A_50",
				Version = 1,
				Name = "Live_A_50",
				ShapeGoal = 2,
				TimeGoal = 5f,
				Hint = StringId.S_050,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			52,
			new LevelInfo
			{
				File = "L0036",
				Key = "A_40",
				Version = 1,
				Name = "Live_A_40",
				ShapeGoal = 3,
				TimeGoal = 16f,
				Hint = StringId.S_040,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			53,
			new LevelInfo
			{
				File = "L0073",
				Key = "A_37",
				Version = 1,
				Name = "Live_A_37",
				ShapeGoal = 2,
				TimeGoal = 5f,
				Hint = StringId.S_037,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			54,
			new LevelInfo
			{
				File = "L0028",
				Key = "A_30",
				Version = 1,
				Name = "Live_A_30",
				ShapeGoal = 3,
				TimeGoal = 11f,
				Hint = StringId.S_030,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			55,
			new LevelInfo
			{
				File = "L0032",
				Key = "A_34",
				Version = 2,
				Name = "Live_A_34",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			56,
			new LevelInfo
			{
				File = "L0030",
				Key = "A_32",
				Version = 2,
				Name = "Live_A_32",
				ShapeGoal = 2,
				TimeGoal = 20f,
				Hint = StringId.S_032,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			57,
			new LevelInfo
			{
				File = "L0034",
				Key = "A_36",
				Version = 1,
				Name = "Live_A_36",
				ShapeGoal = 5,
				TimeGoal = 40f,
				Hint = StringId.S_BALL_TOUCH_LEFTRIGHT,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			58,
			new LevelInfo
			{
				File = "L0024",
				Key = "A_26",
				Version = 1,
				Name = "Live_A_26",
				ShapeGoal = 5,
				TimeGoal = 22f,
				Hint = StringId.S_026,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			59,
			new LevelInfo
			{
				File = "L0021",
				Key = "A_23",
				Version = 1,
				Name = "Live_A_23",
				ShapeGoal = 2,
				TimeGoal = 16f,
				Hint = StringId.S_023,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			60,
			new LevelInfo
			{
				File = "L0037",
				Key = "A_41",
				Version = 1,
				Name = "Live_A_41",
				ShapeGoal = 3,
				TimeGoal = 20f,
				Hint = StringId.S_041,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			61,
			new LevelInfo
			{
				File = "L0060",
				Key = "A_65",
				Version = 1,
				Name = "Live_A_65",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_065,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			62,
			new LevelInfo
			{
				File = "L0063",
				Key = "A_68",
				Version = 1,
				Name = "Live_A_68",
				ShapeGoal = 1,
				TimeGoal = 16f,
				Hint = StringId.S_068,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			63,
			new LevelInfo
			{
				File = "L0056",
				Key = "A_61",
				Version = 1,
				Name = "Live_A_61",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_061,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			64,
			new LevelInfo
			{
				File = "L0050",
				Key = "A_55",
				Version = 1,
				Name = "Live_A_55",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_055,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			65,
			new LevelInfo
			{
				File = "L0049",
				Key = "A_54",
				Version = 1,
				Name = "Live_A_54",
				ShapeGoal = 1,
				TimeGoal = 12f,
				Hint = StringId.S_054,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			66,
			new LevelInfo
			{
				File = "L0048",
				Key = "A_53",
				Version = 1,
				Name = "Live_A_53",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			67,
			new LevelInfo
			{
				File = "L0047",
				Key = "A_52",
				Version = 1,
				Name = "Live_A_52",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_052,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			68,
			new LevelInfo
			{
				File = "L0046",
				Key = "A_51",
				Version = 1,
				Name = "Live_A_51",
				ShapeGoal = 3,
				TimeGoal = 12f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			69,
			new LevelInfo
			{
				File = "L0053",
				Key = "A_58",
				Version = 1,
				Name = "Live_A_58",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_058,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			70,
			new LevelInfo
			{
				File = "L0055",
				Key = "A_60",
				Version = 1,
				Name = "Live_A_60",
				ShapeGoal = 2,
				TimeGoal = 13f,
				Hint = StringId.S_THREEBALLS_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			71,
			new LevelInfo
			{
				File = "L0052",
				Key = "A_57",
				Version = 1,
				Name = "Live_A_57",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_057,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			72,
			new LevelInfo
			{
				File = "L0051",
				Key = "A_56",
				Version = 1,
				Name = "Live_A_56",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_056,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			73,
			new LevelInfo
			{
				File = "L0054",
				Key = "A_59",
				Version = 1,
				Name = "Live_A_59",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_059,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			74,
			new LevelInfo
			{
				File = "L0058",
				Key = "A_63",
				Version = 1,
				Name = "Live_A_63",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_SHAPE_IN_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			75,
			new LevelInfo
			{
				File = "L0057",
				Key = "A_62",
				Version = 1,
				Name = "Live_A_62",
				ShapeGoal = 3,
				TimeGoal = 11f,
				Hint = StringId.S_062,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			76,
			new LevelInfo
			{
				File = "L0059",
				Key = "A_64",
				Version = 1,
				Name = "Live_A_64",
				ShapeGoal = 3,
				TimeGoal = 12f,
				Hint = StringId.S_OBJECT_TOUCH_LEFTRIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			77,
			new LevelInfo
			{
				File = "L0065",
				Key = "A_70",
				Version = 1,
				Name = "Live_A_70",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_070,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			78,
			new LevelInfo
			{
				File = "L0064",
				Key = "A_69",
				Version = 1,
				Name = "Live_A_69",
				ShapeGoal = 1,
				TimeGoal = 14f,
				Hint = StringId.S_069,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			79,
			new LevelInfo
			{
				File = "L0062",
				Key = "A_67",
				Version = 1,
				Name = "Live_A_67",
				ShapeGoal = 4,
				TimeGoal = 30f,
				Hint = StringId.S_THREEBALLS_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			80,
			new LevelInfo
			{
				File = "L0042",
				Key = "A_46",
				Version = 1,
				Name = "Live_A_46",
				ShapeGoal = 2,
				TimeGoal = 18f,
				Hint = StringId.S_046,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			81,
			new LevelInfo
			{
				File = "L0078",
				Key = "A_77",
				Version = 1,
				Name = "Live_A_77",
				ShapeGoal = 1,
				TimeGoal = 13f,
				Hint = StringId.S_077,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			82,
			new LevelInfo
			{
				File = "L0085",
				Key = "A_84",
				Version = 1,
				Name = "Live_A_84",
				ShapeGoal = 2,
				TimeGoal = 15f,
				Hint = StringId.S_041,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			83,
			new LevelInfo
			{
				File = "L0066",
				Key = "A_71",
				Version = 1,
				Name = "Live_A_71",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_ALL_WALLS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			84,
			new LevelInfo
			{
				File = "L0068",
				Key = "A_73",
				Version = 1,
				Name = "Live_A_73",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_073,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			85,
			new LevelInfo
			{
				File = "L0070",
				Key = "A_75",
				Version = 1,
				Name = "Live_A_75",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			86,
			new LevelInfo
			{
				File = "L0077",
				Key = "A_76",
				Version = 1,
				Name = "Live_A_76",
				ShapeGoal = 1,
				TimeGoal = 20f,
				Hint = StringId.S_076,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			87,
			new LevelInfo
			{
				File = "L0081",
				Key = "A_79",
				Version = 1,
				Name = "Live_A_79",
				ShapeGoal = 3,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			88,
			new LevelInfo
			{
				File = "L0067",
				Key = "A_72",
				Version = 1,
				Name = "Live_A_72",
				ShapeGoal = 4,
				TimeGoal = 20f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			89,
			new LevelInfo
			{
				File = "L0069",
				Key = "A_74",
				Version = 1,
				Name = "Live_A_74",
				ShapeGoal = 2,
				TimeGoal = 13f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			90,
			new LevelInfo
			{
				File = "L0079",
				Key = "A_78",
				Version = 1,
				Name = "Live_A_78",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_GLASS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			91,
			new LevelInfo
			{
				File = "L0086",
				Key = "A_85",
				Version = 1,
				Name = "Live_A_85",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_041,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			92,
			new LevelInfo
			{
				File = "L0082",
				Key = "A_81",
				Version = 1,
				Name = "Live_A_81",
				ShapeGoal = 4,
				TimeGoal = 20f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			93,
			new LevelInfo
			{
				File = "L0083",
				Key = "A_82",
				Version = 1,
				Name = "Live_A_82",
				ShapeGoal = 2,
				TimeGoal = 18f,
				Hint = StringId.S_041,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			94,
			new LevelInfo
			{
				File = "L0084",
				Key = "A_83",
				Version = 1,
				Name = "Live_A_83",
				ShapeGoal = 3,
				TimeGoal = 20f,
				Hint = StringId.S_041,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			95,
			new LevelInfo
			{
				File = "L0061",
				Key = "A_66",
				Version = 1,
				Name = "Live_A_66",
				ShapeGoal = 4,
				TimeGoal = 20f,
				Hint = StringId.S_SORT_THE_BALLS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			96,
			new LevelInfo
			{
				File = "L0089",
				Key = "A_89",
				Version = 1,
				Name = "Live_A_89",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			97,
			new LevelInfo
			{
				File = "L0088",
				Key = "A_87",
				Version = 1,
				Name = "Live_A_87",
				ShapeGoal = 3,
				TimeGoal = 15f,
				Hint = StringId.S_031,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			98,
			new LevelInfo
			{
				File = "L0090",
				Key = "A_88",
				Version = 1,
				Name = "Live_A_88",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_031,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			99,
			new LevelInfo
			{
				File = "L0087",
				Key = "A_86",
				Version = 1,
				Name = "Live_A_86",
				ShapeGoal = 3,
				TimeGoal = 14f,
				Hint = StringId.S_031,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			100,
			new LevelInfo
			{
				File = "L0080",
				Key = "A_80",
				Version = 1,
				Name = "Live_A_80",
				ShapeGoal = 6,
				TimeGoal = 30f,
				Hint = StringId.S_080,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			101,
			new LevelInfo
			{
				File = "L0109",
				Key = "A_108",
				Version = 1,
				Name = "Live_A_108",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			102,
			new LevelInfo
			{
				File = "L0110",
				Key = "A_109",
				Version = 1,
				Name = "Live_A_109",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			103,
			new LevelInfo
			{
				File = "L0111",
				Key = "A_110",
				Version = 1,
				Name = "Live_A_110",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			104,
			new LevelInfo
			{
				File = "L0112",
				Key = "A_111",
				Version = 1,
				Name = "Live_A_111",
				ShapeGoal = 4,
				TimeGoal = 23f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			105,
			new LevelInfo
			{
				File = "L0102",
				Key = "A_101",
				Version = 1,
				Name = "Live_A_101",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_077,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			106,
			new LevelInfo
			{
				File = "L0103",
				Key = "A_102",
				Version = 1,
				Name = "Live_A_102",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_023,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			107,
			new LevelInfo
			{
				File = "L0104",
				Key = "A_103",
				Version = 1,
				Name = "Live_A_103",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			108,
			new LevelInfo
			{
				File = "L0106",
				Key = "A_105",
				Version = 1,
				Name = "Live_A_105",
				ShapeGoal = 4,
				TimeGoal = 13f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			109,
			new LevelInfo
			{
				File = "L0107",
				Key = "A_106",
				Version = 1,
				Name = "Live_A_106",
				ShapeGoal = 1,
				TimeGoal = 12f,
				Hint = StringId.S_SHAPE_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			110,
			new LevelInfo
			{
				File = "L0108",
				Key = "A_107",
				Version = 1,
				Name = "Live_A_107",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			111,
			new LevelInfo
			{
				File = "L0105",
				Key = "A_104",
				Version = 1,
				Name = "Live_A_104",
				ShapeGoal = 2,
				TimeGoal = 30f,
				Hint = StringId.S_073,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			112,
			new LevelInfo
			{
				File = "L0113",
				Key = "A_112",
				Version = 1,
				Name = "Live_A_112",
				ShapeGoal = 2,
				TimeGoal = 25f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			113,
			new LevelInfo
			{
				File = "L0114",
				Key = "A_113",
				Version = 1,
				Name = "Live_A_113",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_DRAW_A_SHAPE,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			114,
			new LevelInfo
			{
				File = "L0115",
				Key = "A_114",
				Version = 1,
				Name = "Live_A_114",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			115,
			new LevelInfo
			{
				File = "L0116",
				Key = "A_115",
				Version = 1,
				Name = "Live_A_115",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			116,
			new LevelInfo
			{
				File = "L0117",
				Key = "A_116",
				Version = 1,
				Name = "Live_A_116",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_SHAPE_TOUCH_CEILING,
				StartActive = true,
				Pen = LevelPenType.Floaty
			}
		},
		{
			117,
			new LevelInfo
			{
				File = "L0118",
				Key = "A_117",
				Version = 1,
				Name = "Live_A_117",
				ShapeGoal = 2,
				TimeGoal = 17f,
				Hint = StringId.S_SHAPE_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			118,
			new LevelInfo
			{
				File = "L0119",
				Key = "A_118",
				Version = 1,
				Name = "Live_A_118",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_DRAW_A_SHAPE,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			119,
			new LevelInfo
			{
				File = "L0120",
				Key = "A_119",
				Version = 1,
				Name = "Live_A_119",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			120,
			new LevelInfo
			{
				File = "L0121",
				Key = "A_120",
				Version = 1,
				Name = "Live_A_120",
				ShapeGoal = 1,
				TimeGoal = 14f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			121,
			new LevelInfo
			{
				File = "L0141",
				Key = "A_140",
				Version = 1,
				Name = "Live_A_140",
				ShapeGoal = 1,
				TimeGoal = 15f,
				Hint = StringId.S_SORT_THE_BALLS,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			122,
			new LevelInfo
			{
				File = "L0122",
				Key = "A_121",
				Version = 1,
				Name = "Live_A_121",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			123,
			new LevelInfo
			{
				File = "L0123",
				Key = "A_122",
				Version = 1,
				Name = "Live_A_122",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			124,
			new LevelInfo
			{
				File = "L0124",
				Key = "A_123",
				Version = 1,
				Name = "Live_A_123",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			125,
			new LevelInfo
			{
				File = "L0125",
				Key = "A_124",
				Version = 1,
				Name = "Live_A_124",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			126,
			new LevelInfo
			{
				File = "L0126",
				Key = "A_125",
				Version = 1,
				Name = "Live_A_125",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			127,
			new LevelInfo
			{
				File = "L0127",
				Key = "A_126",
				Version = 1,
				Name = "Live_A_126",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_024,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			128,
			new LevelInfo
			{
				File = "L0128",
				Key = "A_127",
				Version = 1,
				Name = "Live_A_127",
				ShapeGoal = 1,
				TimeGoal = 12f,
				Hint = StringId.S_005,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			129,
			new LevelInfo
			{
				File = "L0129",
				Key = "A_128",
				Version = 1,
				Name = "Live_A_128",
				ShapeGoal = 3,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			130,
			new LevelInfo
			{
				File = "L0130",
				Key = "A_129",
				Version = 1,
				Name = "Live_A_129",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			131,
			new LevelInfo
			{
				File = "L0131",
				Key = "A_130",
				Version = 1,
				Name = "Live_A_130",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			132,
			new LevelInfo
			{
				File = "L0132",
				Key = "A_131",
				Version = 1,
				Name = "Live_A_131",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = true,
				Pen = LevelPenType.Floaty
			}
		},
		{
			133,
			new LevelInfo
			{
				File = "L0133",
				Key = "A_132",
				Version = 1,
				Name = "Live_A_132",
				ShapeGoal = 2,
				TimeGoal = 16f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			134,
			new LevelInfo
			{
				File = "L0134",
				Key = "A_133",
				Version = 1,
				Name = "Live_A_133",
				ShapeGoal = 3,
				TimeGoal = 10f,
				Hint = StringId.S_GET_ORANGEBALL_OUT_OF_BOX,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			135,
			new LevelInfo
			{
				File = "L0135",
				Key = "A_134",
				Version = 1,
				Name = "Live_A_134",
				ShapeGoal = 1,
				TimeGoal = 17f,
				Hint = StringId.S_BALLS_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			136,
			new LevelInfo
			{
				File = "L0136",
				Key = "A_135",
				Version = 1,
				Name = "Live_A_135",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			137,
			new LevelInfo
			{
				File = "L0137",
				Key = "A_136",
				Version = 1,
				Name = "Live_A_136",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			138,
			new LevelInfo
			{
				File = "L0138",
				Key = "A_137",
				Version = 1,
				Name = "Live_A_137",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = true,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			139,
			new LevelInfo
			{
				File = "L0140",
				Key = "A_139",
				Version = 1,
				Name = "Live_A_139",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			140,
			new LevelInfo
			{
				File = "L0139",
				Key = "A_138",
				Version = 1,
				Name = "Live_A_138",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			141,
			new LevelInfo
			{
				File = "L0142",
				Key = "A_141",
				Version = 1,
				Name = "Live_A_141",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			142,
			new LevelInfo
			{
				File = "L0143",
				Key = "A_142",
				Version = 1,
				Name = "Live_A_142",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			143,
			new LevelInfo
			{
				File = "L0144",
				Key = "A_143",
				Version = 1,
				Name = "Live_A_143",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_THREEBALLS_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			144,
			new LevelInfo
			{
				File = "L0145",
				Key = "A_144",
				Version = 1,
				Name = "Live_A_144",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_SHAPE_TOUCH_CEILING,
				StartActive = true,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			145,
			new LevelInfo
			{
				File = "L0146",
				Key = "A_145",
				Version = 1,
				Name = "Live_A_145",
				ShapeGoal = 1,
				TimeGoal = 40f,
				Hint = StringId.S_OBJECT_TOUCH_ALL_WALLS,
				StartActive = true,
				Pen = LevelPenType.Red
			}
		},
		{
			146,
			new LevelInfo
			{
				File = "L0147",
				Key = "A_146",
				Version = 1,
				Name = "Live_A_146",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = true,
				Pen = LevelPenType.Floaty
			}
		},
		{
			147,
			new LevelInfo
			{
				File = "L0148",
				Key = "A_147",
				Version = 1,
				Name = "Live_A_147",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = true,
				Pen = LevelPenType.Floaty
			}
		},
		{
			148,
			new LevelInfo
			{
				File = "L0149",
				Key = "A_148",
				Version = 1,
				Name = "Live_A_148",
				ShapeGoal = 3,
				TimeGoal = 30f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			149,
			new LevelInfo
			{
				File = "L0150",
				Key = "A_149",
				Version = 1,
				Name = "Live_A_149",
				ShapeGoal = 2,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = true,
				Pen = LevelPenType.Floaty
			}
		},
		{
			150,
			new LevelInfo
			{
				File = "L0151",
				Key = "A_150",
				Version = 1,
				Name = "Live_A_150",
				ShapeGoal = 1,
				TimeGoal = 18f,
				Hint = StringId.S_SHAPE_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			151,
			new LevelInfo
			{
				File = "L0152",
				Key = "A_151",
				Version = 1,
				Name = "Live_A_151",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			152,
			new LevelInfo
			{
				File = "L0153",
				Key = "A_152",
				Version = 1,
				Name = "Live_A_152",
				ShapeGoal = 1,
				TimeGoal = 12f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = true,
				Pen = LevelPenType.Red
			}
		},
		{
			153,
			new LevelInfo
			{
				File = "L0154",
				Key = "A_153",
				Version = 1,
				Name = "Live_A_153",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_THREEBALLS_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			154,
			new LevelInfo
			{
				File = "L0155",
				Key = "A_154",
				Version = 1,
				Name = "Live_A_154",
				ShapeGoal = 1,
				TimeGoal = 13f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			155,
			new LevelInfo
			{
				File = "L0156",
				Key = "A_155",
				Version = 1,
				Name = "Live_A_155",
				ShapeGoal = 2,
				TimeGoal = 5f,
				Hint = StringId.S_SHAPE_IN_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			156,
			new LevelInfo
			{
				File = "L0158",
				Key = "A_157",
				Version = 1,
				Name = "Live_A_157",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			157,
			new LevelInfo
			{
				File = "L0159",
				Key = "A_158",
				Version = 1,
				Name = "Live_A_158",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_SHAPE_IN_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			158,
			new LevelInfo
			{
				File = "L0160",
				Key = "A_159",
				Version = 1,
				Name = "Live_A_159",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_CLEAR_OBJECTS_FROM_PLATFORM,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			159,
			new LevelInfo
			{
				File = "L0161",
				Key = "A_160",
				Version = 1,
				Name = "Live_A_160",
				ShapeGoal = 1,
				TimeGoal = 14f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			160,
			new LevelInfo
			{
				File = "L0157",
				Key = "A_156",
				Version = 1,
				Name = "Live_A_156",
				ShapeGoal = 5,
				TimeGoal = 20f,
				Hint = StringId.S_SORT_THE_BALLS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			161,
			new LevelInfo
			{
				File = "L0163",
				Key = "A_162",
				Version = 1,
				Name = "Live_A_162",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_LEFTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			162,
			new LevelInfo
			{
				File = "L0164",
				Key = "A_163",
				Version = 1,
				Name = "Live_A_163",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			163,
			new LevelInfo
			{
				File = "L0180",
				Key = "A_179",
				Version = 1,
				Name = "Live_A_179",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_DRAW_A_SHAPE,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			164,
			new LevelInfo
			{
				File = "L0181",
				Key = "A_180",
				Version = 1,
				Name = "Live_A_180",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			165,
			new LevelInfo
			{
				File = "L0167",
				Key = "A_166",
				Version = 1,
				Name = "Live_A_166",
				ShapeGoal = 2,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			166,
			new LevelInfo
			{
				File = "L0168",
				Key = "A_167",
				Version = 1,
				Name = "Live_A_167",
				ShapeGoal = 2,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_TOUCH_ALL_WALLS,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			167,
			new LevelInfo
			{
				File = "L0169",
				Key = "A_168",
				Version = 1,
				Name = "Live_A_168",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_ALL_WALLS,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			168,
			new LevelInfo
			{
				File = "L0170",
				Key = "A_169",
				Version = 1,
				Name = "Live_A_169",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			169,
			new LevelInfo
			{
				File = "L0171",
				Key = "A_170",
				Version = 1,
				Name = "Live_A_170",
				ShapeGoal = 3,
				TimeGoal = 20f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Bouncy
			}
		},
		{
			170,
			new LevelInfo
			{
				File = "L0172",
				Key = "A_171",
				Version = 1,
				Name = "Live_A_171",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			171,
			new LevelInfo
			{
				File = "L0173",
				Key = "A_172",
				Version = 1,
				Name = "Live_A_172",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			172,
			new LevelInfo
			{
				File = "L0174",
				Key = "A_173",
				Version = 1,
				Name = "Live_A_173",
				ShapeGoal = 1,
				TimeGoal = 17f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			173,
			new LevelInfo
			{
				File = "L0175",
				Key = "A_174",
				Version = 1,
				Name = "Live_A_174",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			174,
			new LevelInfo
			{
				File = "L0176",
				Key = "A_175",
				Version = 1,
				Name = "Live_A_175",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			175,
			new LevelInfo
			{
				File = "L0177",
				Key = "A_176",
				Version = 1,
				Name = "Live_A_176",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_TOUCH_LEFTRIGHT,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			176,
			new LevelInfo
			{
				File = "L0178",
				Key = "A_177",
				Version = 1,
				Name = "Live_A_177",
				ShapeGoal = 1,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_INSIDE_GLASS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			177,
			new LevelInfo
			{
				File = "L0179",
				Key = "A_178",
				Version = 1,
				Name = "Live_A_178",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			178,
			new LevelInfo
			{
				File = "L0182",
				Key = "A_181",
				Version = 1,
				Name = "Live_A_181",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			179,
			new LevelInfo
			{
				File = "L0183",
				Key = "A_182",
				Version = 1,
				Name = "Live_A_182",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_SHAPE_IN_GLASS,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			180,
			new LevelInfo
			{
				File = "L0184",
				Key = "A_183",
				Version = 1,
				Name = "Live_A_183",
				ShapeGoal = 6,
				TimeGoal = 16f,
				Hint = StringId.S_SORT_THE_BALLS,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			181,
			new LevelInfo
			{
				File = "L0185",
				Key = "A_184",
				Version = 1,
				Name = "Live_A_184",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			182,
			new LevelInfo
			{
				File = "L0186",
				Key = "A_185",
				Version = 1,
				Name = "Live_A_185",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			183,
			new LevelInfo
			{
				File = "L0188",
				Key = "A_187",
				Version = 1,
				Name = "Live_A_187",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			184,
			new LevelInfo
			{
				File = "L0187",
				Key = "A_186",
				Version = 1,
				Name = "Live_A_186",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			185,
			new LevelInfo
			{
				File = "L0189",
				Key = "A_188",
				Version = 1,
				Name = "Live_A_188",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			186,
			new LevelInfo
			{
				File = "L0190",
				Key = "A_189",
				Version = 1,
				Name = "Live_A_189",
				ShapeGoal = 2,
				TimeGoal = 9f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Floaty
			}
		},
		{
			187,
			new LevelInfo
			{
				File = "L0191",
				Key = "A_190",
				Version = 1,
				Name = "Live_A_190",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			188,
			new LevelInfo
			{
				File = "L0194",
				Key = "A_193",
				Version = 1,
				Name = "Live_A_193",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			189,
			new LevelInfo
			{
				File = "L0193",
				Key = "A_192",
				Version = 1,
				Name = "Live_A_192",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			190,
			new LevelInfo
			{
				File = "L0196",
				Key = "A_195",
				Version = 1,
				Name = "Live_A_195",
				ShapeGoal = 4,
				TimeGoal = 15f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			191,
			new LevelInfo
			{
				File = "L0195",
				Key = "A_194",
				Version = 1,
				Name = "Live_A_194",
				ShapeGoal = 2,
				TimeGoal = 4f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			192,
			new LevelInfo
			{
				File = "L0197",
				Key = "A_196",
				Version = 1,
				Name = "Live_A_196",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			193,
			new LevelInfo
			{
				File = "L0198",
				Key = "A_197",
				Version = 1,
				Name = "Live_A_197",
				ShapeGoal = 3,
				TimeGoal = 70f,
				Hint = StringId.S_THREEBALLS_INSIDE_ORANGE_BOX,
				StartActive = true,
				Pen = LevelPenType.Normal
			}
		},
		{
			194,
			new LevelInfo
			{
				File = "L0199",
				Key = "A_198",
				Version = 1,
				Name = "Live_A_198",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			195,
			new LevelInfo
			{
				File = "L0204",
				Key = "A_203",
				Version = 1,
				Name = "Live_A_203",
				ShapeGoal = 3,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			196,
			new LevelInfo
			{
				File = "L0200",
				Key = "A_199",
				Version = 1,
				Name = "Live_A_199",
				ShapeGoal = 3,
				TimeGoal = 25f,
				Hint = StringId.S_GET_BALL_OUT_OF_SHAPE,
				StartActive = true,
				Pen = LevelPenType.Icy
			}
		},
		{
			197,
			new LevelInfo
			{
				File = "L0201",
				Key = "A_200",
				Version = 1,
				Name = "Live_A_200",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_MAGNET_TOUCH,
				StartActive = true,
				Pen = LevelPenType.Icy
			}
		},
		{
			198,
			new LevelInfo
			{
				File = "L0202",
				Key = "A_201",
				Version = 1,
				Name = "Live_A_201",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			199,
			new LevelInfo
			{
				File = "L0203",
				Key = "A_202",
				Version = 1,
				Name = "Live_A_202",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_GET_BALL_OUT_OF_SHAPE,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			200,
			new LevelInfo
			{
				File = "L0192",
				Key = "A_191",
				Version = 1,
				Name = "Live_A_191",
				ShapeGoal = 2,
				TimeGoal = 20f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Icy
			}
		},
		{
			201,
			new LevelInfo
			{
				File = "L0205",
				Key = "A_204",
				Version = 1,
				Name = "Live_A_204",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			202,
			new LevelInfo
			{
				File = "L0208",
				Key = "A_207",
				Version = 1,
				Name = "Live_A_207",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			203,
			new LevelInfo
			{
				File = "L0207",
				Key = "A_206",
				Version = 1,
				Name = "Live_A_206",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			204,
			new LevelInfo
			{
				File = "L0209",
				Key = "A_208",
				Version = 1,
				Name = "Live_A_208",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			205,
			new LevelInfo
			{
				File = "L0206",
				Key = "A_205",
				Version = 1,
				Name = "Live_A_205",
				ShapeGoal = 1,
				TimeGoal = 14f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			206,
			new LevelInfo
			{
				File = "L0210",
				Key = "A_209",
				Version = 1,
				Name = "Live_A_209",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_LEFTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			207,
			new LevelInfo
			{
				File = "L0211",
				Key = "A_210",
				Version = 1,
				Name = "Live_A_210",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			208,
			new LevelInfo
			{
				File = "L0212",
				Key = "A_211",
				Version = 1,
				Name = "Live_A_211",
				ShapeGoal = 2,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			209,
			new LevelInfo
			{
				File = "L0214",
				Key = "A_213",
				Version = 1,
				Name = "Live_A_213",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			210,
			new LevelInfo
			{
				File = "L0213",
				Key = "A_212",
				Version = 1,
				Name = "Live_A_212",
				ShapeGoal = 2,
				TimeGoal = 13f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			211,
			new LevelInfo
			{
				File = "L0215",
				Key = "A_214",
				Version = 1,
				Name = "Live_A_214",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			212,
			new LevelInfo
			{
				File = "L0217",
				Key = "A_216",
				Version = 1,
				Name = "Live_A_216",
				ShapeGoal = 2,
				TimeGoal = 6f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			213,
			new LevelInfo
			{
				File = "L0216",
				Key = "A_215",
				Version = 1,
				Name = "Live_A_215",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			214,
			new LevelInfo
			{
				File = "L0219",
				Key = "A_218",
				Version = 1,
				Name = "Live_A_218",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			215,
			new LevelInfo
			{
				File = "L0221",
				Key = "A_220",
				Version = 1,
				Name = "Live_A_220",
				ShapeGoal = 4,
				TimeGoal = 25f,
				Hint = StringId.S_SORT_THE_BALLS,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			216,
			new LevelInfo
			{
				File = "L0220",
				Key = "A_219",
				Version = 1,
				Name = "Live_A_219",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			217,
			new LevelInfo
			{
				File = "L0222",
				Key = "A_221",
				Version = 1,
				Name = "Live_A_221",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALLS_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			218,
			new LevelInfo
			{
				File = "L0223",
				Key = "A_222",
				Version = 1,
				Name = "Live_A_222",
				ShapeGoal = 2,
				TimeGoal = 12f,
				Hint = StringId.S_BALL_TOUCH_RIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			219,
			new LevelInfo
			{
				File = "L0224",
				Key = "A_223",
				Version = 1,
				Name = "Live_A_223",
				ShapeGoal = 3,
				TimeGoal = 16f,
				Hint = StringId.S_BALL_TOUCH_LEFTRIGHT,
				StartActive = false,
				Pen = LevelPenType.Normal
			}
		},
		{
			220,
			new LevelInfo
			{
				File = "L0218",
				Key = "A_217",
				Version = 1,
				Name = "Live_A_217",
				ShapeGoal = 3,
				TimeGoal = 12f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			221,
			new LevelInfo
			{
				File = "L0225",
				Key = "A_224",
				Version = 1,
				Name = "Live_A_224",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			222,
			new LevelInfo
			{
				File = "L0226",
				Key = "A_225",
				Version = 1,
				Name = "Live_A_225",
				ShapeGoal = 1,
				TimeGoal = 8f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			223,
			new LevelInfo
			{
				File = "L0227",
				Key = "A_226",
				Version = 1,
				Name = "Live_A_226",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			224,
			new LevelInfo
			{
				File = "L0228",
				Key = "A_227",
				Version = 1,
				Name = "Live_A_227",
				ShapeGoal = 2,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			225,
			new LevelInfo
			{
				File = "L0229",
				Key = "A_228",
				Version = 1,
				Name = "Live_A_228",
				ShapeGoal = 1,
				TimeGoal = 20f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			226,
			new LevelInfo
			{
				File = "L0230",
				Key = "A_229",
				Version = 1,
				Name = "Live_A_229",
				ShapeGoal = 2,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			227,
			new LevelInfo
			{
				File = "L0231",
				Key = "A_230",
				Version = 1,
				Name = "Live_A_230",
				ShapeGoal = 1,
				TimeGoal = 5f,
				Hint = StringId.S_OBJECT_TOUCH_LEFTRIGHTWALL,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			228,
			new LevelInfo
			{
				File = "L0232",
				Key = "A_231",
				Version = 1,
				Name = "Live_A_231",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			229,
			new LevelInfo
			{
				File = "L0233",
				Key = "A_232",
				Version = 1,
				Name = "Live_A_232",
				ShapeGoal = 2,
				TimeGoal = 25f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			230,
			new LevelInfo
			{
				File = "L0234",
				Key = "A_233",
				Version = 1,
				Name = "Live_A_233",
				ShapeGoal = 3,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			231,
			new LevelInfo
			{
				File = "L0235",
				Key = "A_234",
				Version = 1,
				Name = "Live_A_234",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			232,
			new LevelInfo
			{
				File = "L0236",
				Key = "A_235",
				Version = 1,
				Name = "Live_A_235",
				ShapeGoal = 3,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			233,
			new LevelInfo
			{
				File = "L0237",
				Key = "A_236",
				Version = 1,
				Name = "Live_A_236",
				ShapeGoal = 1,
				TimeGoal = 10f,
				Hint = StringId.S_OBJECT_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			234,
			new LevelInfo
			{
				File = "L0238",
				Key = "A_237",
				Version = 1,
				Name = "Live_A_237",
				ShapeGoal = 1,
				TimeGoal = 7f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			235,
			new LevelInfo
			{
				File = "L0239",
				Key = "A_238",
				Version = 1,
				Name = "Live_A_238",
				ShapeGoal = 1,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			236,
			new LevelInfo
			{
				File = "L0240",
				Key = "A_239",
				Version = 1,
				Name = "Live_A_239",
				ShapeGoal = 1,
				TimeGoal = 6f,
				Hint = StringId.S_BALL_TOUCH_CEILING,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			237,
			new LevelInfo
			{
				File = "L0241",
				Key = "A_240",
				Version = 1,
				Name = "Live_A_240",
				ShapeGoal = 3,
				TimeGoal = 15f,
				Hint = StringId.S_BALL_TOUCH_GROUND,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			238,
			new LevelInfo
			{
				File = "L0242",
				Key = "A_241",
				Version = 1,
				Name = "Live_A_241",
				ShapeGoal = 1,
				TimeGoal = 14f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			239,
			new LevelInfo
			{
				File = "L0243",
				Key = "A_242",
				Version = 1,
				Name = "Live_A_242",
				ShapeGoal = 2,
				TimeGoal = 6f,
				Hint = StringId.S_OBJECT_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		},
		{
			240,
			new LevelInfo
			{
				File = "L0244",
				Key = "A_243",
				Version = 1,
				Name = "Live_A_243",
				ShapeGoal = 3,
				TimeGoal = 17f,
				Hint = StringId.S_BALL_INSIDE_ORANGE_BOX,
				StartActive = false,
				Pen = LevelPenType.Red
			}
		}
	};

	public static int BackgroundIndex = 0;

	public static bool CommunityLevel = false;

	private static int m_levelLoaded = 0;

	private static bool s_tryLevel = false;

	public int PackCount
	{
		get
		{
			return s_levelPacks.Count;
		}
	}

	public static int Level { get; set; }

	public static GameObject LevelParent { get; private set; }

	public static bool IsTryLevel
	{
		get
		{
			return s_tryLevel;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		Level = 1;
	}

	private IEnumerator Start()
	{
		if (DataStore.Instance.LevelsLocked.Count == 0)
		{
			LockLevels();
		}
		else
		{
			CleanupLevelLocks();
		}
		if (DataStore.Purge())
		{
			DataStore.Save();
		}
		string assetBundlePath = Application.dataPath + "!assets/AssetBundles/levels/seta";
		if (File.Exists(assetBundlePath))
		{
			bundleLoadRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
			yield return bundleLoadRequest;
			m_bundleLevels = bundleLoadRequest.assetBundle;
		}
		yield return 1;
	}

	private void Update()
	{
		if (m_levelLoaded <= 0)
		{
			return;
		}
		m_levelLoaded--;
		if (m_levelLoaded == 0)
		{
			LevelParent = GameObject.Find("Level");
			if (LevelParent != null)
			{
				LevelParent.transform.parent = TouchDrawPhysics.Instance.LevelParent.transform;
				LevelParent.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
			TouchDrawPhysics.Instance.SetShapeMaterial(GetPenType());
		}
	}

	public void LockLevels()
	{
		DataStore.Instance.LevelsLocked.Clear();
		foreach (LevelInfo levelInfo in getLevelInfos())
		{
			DataStore.Instance.LevelsLocked.Add(levelInfo.Key, true);
		}
		unlockFirstRow();
	}

	private void unlockFirstRow()
	{
		for (int i = 1; i <= s_levelGroupCount; i++)
		{
			LockLevel(i, false);
		}
	}

	public void UnlockFirstRowOfPack(PurchasableItem pack)
	{
		if (pack == PurchasableItem.NO_ADS)
		{
			return;
		}
		int num = 0;
		foreach (LevelPack s_levelPack in s_levelPacks)
		{
			if (s_levelPack.Item == pack)
			{
				num = s_levelPack.FirstLevel;
			}
		}
		for (int i = num; i < num + s_levelGroupCount; i++)
		{
			DataStore.Instance.LevelsLocked[getLevelInfo(i).Key] = false;
		}
	}

	public void UnlockAllLevelsInPack(PurchasableItem pack)
	{
		int num = 0;
		int num2 = 0;
		foreach (LevelPack item in s_levelPacks.Union(s_gamePacks))
		{
			if (item.Item == pack)
			{
				num = item.FirstLevel;
				num2 = item.LastLevel;
			}
		}
		if (num != num2)
		{
			for (int i = num; i <= num2; i++)
			{
				DataStore.Instance.LevelsLocked[s_levelInfo[i].Key] = false;
			}
		}
	}

	public int GetStarsEarnedInPack(int packIndex)
	{
		int num = 0;
		LevelPack levelPack = s_levelPacks[packIndex];
		for (int i = levelPack.FirstLevel; i <= levelPack.LastLevel; i++)
		{
			num += getStarCount(GetLevelCompletion(i));
		}
		return num;
	}

	public int GetStarsForGift(int packIndex)
	{
		if (packIndex >= 0 && packIndex < s_levelPacks.Count)
		{
			int packUnlockStars = DataStore.Instance.ConfigSettings.GetPackUnlockStars(packIndex - 1);
			if (packUnlockStars != -1)
			{
				return packUnlockStars;
			}
			return s_levelPacks[packIndex].StarsForGift;
		}
		return 0;
	}

	public void UnlockLevels()
	{
		DataStore.Instance.LevelsLocked.Clear();
		foreach (LevelInfo levelInfo in getLevelInfos())
		{
			if (!DataStore.Instance.LevelsLocked.ContainsKey(levelInfo.Key))
			{
				DataStore.Instance.LevelsLocked.Add(levelInfo.Key, false);
			}
			else
			{
				DataStore.Instance.LevelsLocked[levelInfo.Key] = false;
			}
		}
	}

	public bool UnlockLevelGroup(int level, int groupModifier = 0)
	{
		int num = (level - 1) / s_levelGroupCount;
		int num2 = num * s_levelGroupCount + 1;
		int num3 = (num + 1) * s_levelGroupCount;
		int levelCount = GetLevelCount();
		int num4 = 0;
		for (int i = num2; i <= num3; i++)
		{
			if (i <= levelCount && GetLevelCompletion(i) > LevelCompletion.Unsolved)
			{
				num4++;
			}
		}
		if (num4 >= s_levelsSolvedToUnlockGroup)
		{
			int num5 = num + groupModifier;
			num2 = num5 * s_levelGroupCount + 1;
			num3 = (num5 + 1) * s_levelGroupCount;
			for (int j = num2; j <= num3; j++)
			{
				if (j <= levelCount)
				{
					LockLevel(j, false);
				}
			}
			return true;
		}
		return false;
	}

	public void CleanupLevelLocks()
	{
		unlockFirstRow();
		int levelCount = GetLevelCount();
		int num = levelCount / s_levelGroupCount;
		for (int i = 1; i < num; i++)
		{
			int groupLockCount = getGroupLockCount(i);
			if (groupLockCount <= 0 || groupLockCount >= s_levelGroupCount)
			{
				continue;
			}
			int groupSolvedCount = getGroupSolvedCount(i - 1);
			bool lockLevel = true;
			if (groupSolvedCount >= s_levelsSolvedToUnlockGroup)
			{
				lockLevel = false;
			}
			int num2 = i * s_levelGroupCount + 1;
			int num3 = (i + 1) * s_levelGroupCount;
			for (int j = num2; j <= num3; j++)
			{
				if (j <= levelCount)
				{
					LockLevel(j, lockLevel);
				}
			}
		}
	}

	private int getGroupSolvedCount(int group)
	{
		int num = group * s_levelGroupCount + 1;
		int num2 = (group + 1) * s_levelGroupCount;
		int levelCount = GetLevelCount();
		int num3 = 0;
		for (int i = num; i <= num2; i++)
		{
			if (i <= levelCount && GetLevelCompletion(i) > LevelCompletion.Unsolved)
			{
				num3++;
			}
		}
		return num3;
	}

	private int getGroupLockCount(int group)
	{
		int num = group * s_levelGroupCount + 1;
		int num2 = (group + 1) * s_levelGroupCount;
		int levelCount = GetLevelCount();
		int num3 = 0;
		for (int i = num; i <= num2; i++)
		{
			if (i <= levelCount && GetLevelLocked(i))
			{
				num3++;
			}
		}
		return num3;
	}

	public void LockLevel(int level, bool lockLevel = true)
	{
		string key = getLevelInfo(level).Key;
		if (DataStore.Instance.LevelsLocked.ContainsKey(key))
		{
			DataStore.Instance.LevelsLocked[key] = lockLevel;
		}
		else
		{
			DataStore.Instance.LevelsLocked.Add(key, lockLevel);
		}
	}

	public string GetLevelName()
	{
		return getLevelInfo(Level).Name;
	}

	public int GetLevelVersion()
	{
		return getLevelInfo(Level).Version;
	}

	public bool IsLevelNew(int level)
	{
		return getLevelInfo(level).New;
	}

	public static LevelPenType GetLevelPen(int level)
	{
		return getLevelInfo(level).Pen;
	}

	public CollisionDetectionMode2D GetCollisionMode()
	{
		if (CommunityLevel)
		{
			return CollisionDetectionMode2D.None;
		}
		List<string> list = new List<string>();
		list.Add("A_64");
		list.Add("A_125");
		List<string> list2 = list;
		string key = getLevelInfo(Level).Key;
		if (list2.Contains(key))
		{
			return CollisionDetectionMode2D.Continuous;
		}
		return CollisionDetectionMode2D.None;
	}

	public static HashSet<string> GetAllLevelKeysAsSet()
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (LevelInfo levelInfo in getLevelInfos())
		{
			hashSet.Add(levelInfo.Key);
		}
		return hashSet;
	}

	public static List<string> GetAllLevelKeys()
	{
		List<string> list = new List<string>();
		foreach (LevelInfo levelInfo in getLevelInfos())
		{
			list.Add(levelInfo.Key);
		}
		return list;
	}

	public string GetLevelKey()
	{
		if (CommunityLevel)
		{
			return CommunityManager.CurrentLevelId;
		}
		return getLevelInfo(Level).Key;
	}

	public string GetLevelKey(int level)
	{
		return getLevelInfo(level).Key;
	}

	private static string GetLevelFile(int level)
	{
		return getLevelInfo(level).File;
	}

	public static int GetLevelCount()
	{
		return getLevelInfos().Count();
	}

	public static int GetAvailableLevelCount()
	{
		int num = 0;
		foreach (LevelPack s_levelPack in s_levelPacks)
		{
			if (s_levelPack.Available)
			{
				num += s_levelPack.LastLevel - s_levelPack.FirstLevel + 1;
			}
		}
		return num;
	}

	public static int GetStartLevelFromPack(int packIndex)
	{
		return s_levelPacks[packIndex].FirstLevel;
	}

	public string GetLevelHint()
	{
		if (CommunityLevel)
		{
			return TextLibrary.Get(CommunityManager.CurrentLevel.Goal);
		}
		return TextLibrary.Get(getLevelInfo(Level).Hint);
	}

	public static int GetLevelIDFromKey(string key)
	{
		int result = -1;
		foreach (KeyValuePair<int, LevelInfo> item in s_levelInfo)
		{
			if (item.Value.Key == key)
			{
				result = item.Key;
				break;
			}
		}
		return result;
	}

	public static int GetLevelIDFromFile(string file)
	{
		int result = -1;
		foreach (KeyValuePair<int, LevelInfo> item in s_levelInfo)
		{
			if (item.Value.File == file)
			{
				result = item.Key;
				break;
			}
		}
		return result;
	}

	public void SetAllLevelsComplete()
	{
		foreach (LevelPack s_levelPack in s_levelPacks)
		{
			if (s_levelPack.Available && s_levelPack.CheatUnlocks)
			{
				for (int i = s_levelPack.FirstLevel; i <= s_levelPack.LastLevel; i++)
				{
					Level = i;
					Debug.Log("Solving level: " + Level);
					SetLevelComplete(true, true, true);
				}
			}
		}
		Level = 1;
	}

	public LevelCompletion GetLevelComplete(bool solved, bool timeSolved, bool shapeSolved)
	{
		LevelCompletion levelCompletion = LevelCompletion.Unsolved;
		if (solved)
		{
			levelCompletion |= LevelCompletion.Solved;
		}
		if (timeSolved)
		{
			levelCompletion |= LevelCompletion.TimeSolved;
		}
		if (shapeSolved)
		{
			levelCompletion |= LevelCompletion.ShapeSolved;
		}
		return levelCompletion;
	}

	public LevelCompletion SetLevelComplete(bool solved, bool timeSolved, bool shapeSolved)
	{
		string levelKey = GetLevelKey();
		LevelCompletion levelComplete = GetLevelComplete(solved, timeSolved, shapeSolved);
		if (!CommunityLevel)
		{
			SerializableDictionaryStringInt levelsSolved = DataStore.Instance.LevelsSolved;
			if (!levelsSolved.ContainsKey(levelKey))
			{
				levelsSolved.Add(levelKey, (int)levelComplete);
			}
			else
			{
				LevelCompletion value = (LevelCompletion)((int)levelComplete | levelsSolved[levelKey]);
				levelsSolved[levelKey] = (int)value;
			}
			UnlockLevelGroup(Level, 1);
		}
		return levelComplete;
	}

	public bool SetMinShapeCount(int shapeCount)
	{
		bool result = false;
		string key = getLevelInfo(Level).Key;
		if (!DataStore.Instance.LevelsMinShapeCount.ContainsKey(key))
		{
			DataStore.Instance.LevelsMinShapeCount.Add(key, shapeCount);
			result = true;
		}
		else if (shapeCount < DataStore.Instance.LevelsMinShapeCount[key])
		{
			DataStore.Instance.LevelsMinShapeCount[key] = shapeCount;
			result = true;
		}
		return result;
	}

	public bool SetMinTime(float playTime)
	{
		bool result = false;
		string key = getLevelInfo(Level).Key;
		if (!DataStore.Instance.LevelsMinTime.ContainsKey(key))
		{
			DataStore.Instance.LevelsMinTime.Add(key, playTime);
			result = true;
		}
		else if (playTime < DataStore.Instance.LevelsMinTime[key])
		{
			DataStore.Instance.LevelsMinTime[key] = playTime;
			result = true;
		}
		return result;
	}

	public LevelCompletion GetLevelCompletion()
	{
		return GetLevelCompletion(GetLevelKey());
	}

	public LevelCompletion GetLevelCompletion(string key)
	{
		LevelCompletion levelCompletion = LevelCompletion.Unsolved;
		if (CommunityLevel)
		{
			if (ParseAPI.Instance.IsCommunityLevelAttempted(key))
			{
				levelCompletion |= ParseAPI.Instance.GetCommunityLevelCompletion(key);
			}
		}
		else if (DataStore.Instance.LevelsSolved.ContainsKey(key))
		{
			levelCompletion = (LevelCompletion)((int)levelCompletion | DataStore.Instance.LevelsSolved[key]);
		}
		return levelCompletion;
	}

	public bool IsStartActive()
	{
		if (CommunityLevel)
		{
			return CommunityManager.CurrentLevel.ActiveOnStart;
		}
		return getLevelInfo(Level).StartActive;
	}

	public int CalculateLevelStars(int level, bool solved, int shapeCount, float gameDuration)
	{
		LevelInfo levelInfo = getLevelInfo(level);
		int num = 0;
		int shapeGoal = levelInfo.ShapeGoal;
		float timeGoal = levelInfo.TimeGoal;
		if (solved)
		{
			num++;
			if (shapeCount <= shapeGoal)
			{
				num++;
			}
			if (gameDuration <= timeGoal)
			{
				num++;
			}
		}
		return num;
	}

	public bool IsTimeSolved(float gameDuration)
	{
		return gameDuration <= GetTimeGoal();
	}

	public bool IsShapesSolved(int shapeCount)
	{
		return shapeCount <= GetShapeGoal();
	}

	public float GetTimeGoal()
	{
		if (CommunityLevel)
		{
			return CommunityManager.CurrentLevel.TimeGoal;
		}
		return getLevelInfo(Level).TimeGoal;
	}

	public int GetShapeGoal()
	{
		if (CommunityLevel)
		{
			return CommunityManager.CurrentLevel.ShapeGoal;
		}
		return getLevelInfo(Level).ShapeGoal;
	}

	public LevelPenType GetPenType()
	{
		if (CommunityLevel)
		{
			return CommunityManager.CurrentLevel.PenMaterial;
		}
		return getLevelInfo(Level).Pen;
	}

	public LevelCompletion GetLevelCompletion(int level)
	{
		string key = getLevelInfo(level).Key;
		int value = 0;
		DataStore.Instance.LevelsSolved.TryGetValue(key, out value);
		return (LevelCompletion)value;
	}

	public bool GetLevelLocked(int level)
	{
		string key = getLevelInfo(level).Key;
		if (DataStore.Instance.LevelsLocked.ContainsKey(key))
		{
			return DataStore.Instance.LevelsLocked[key];
		}
		DataStore.Instance.LevelsLocked.Add(key, true);
		return true;
	}

	public int GetLevelPackIndex(int level)
	{
		int result = -1;
		foreach (LevelPack s_levelPack in s_levelPacks)
		{
			if (level >= s_levelPack.FirstLevel && level <= s_levelPack.LastLevel)
			{
				result = s_levelPacks.IndexOf(s_levelPack);
				break;
			}
		}
		return result;
	}

	public bool GetPackAvailable(int pack)
	{
		if (pack > -1 && pack < s_levelPacks.Count)
		{
			return s_levelPacks[pack].Available;
		}
		return false;
	}

	public static LevelPack GetPack(int pack)
	{
		if (pack > -1 && pack < s_levelPacks.Count)
		{
			return s_levelPacks[pack];
		}
		return new LevelPack
		{
			Available = false,
			Free = false,
			Item = PurchasableItem.INVALID,
			StarsForGift = 999
		};
	}

	public static PurchasableItem GetPackItem(int pack)
	{
		if (pack > -1 && pack < s_levelPacks.Count)
		{
			return s_levelPacks[pack].Item;
		}
		return PurchasableItem.INVALID;
	}

	public static LevelPack GetPack(PurchasableItem item)
	{
		foreach (LevelPack s_levelPack in s_levelPacks)
		{
			if (s_levelPack.Item == item)
			{
				return s_levelPack;
			}
		}
		throw new Exception("Invalid purchaseable item.");
	}

	public int GetTotalLevelsSolved()
	{
		return DataStore.Instance.LevelsSolved.Count;
	}

	public int GetTotalLevelsSolved(int minLevel, int maxLevel)
	{
		int num = 0;
		for (int i = minLevel; i <= maxLevel; i++)
		{
			string key = s_levelInfo[i].Key;
			if (DataStore.Instance.LevelsSolved.ContainsKey(key) && DataStore.Instance.LevelsSolved[key] >= 1)
			{
				num++;
			}
		}
		return num;
	}

	public TimeSpan GetTotalTime(int minLevel, int maxLevel)
	{
		float num = 0f;
		for (int i = minLevel; i <= maxLevel; i++)
		{
			string key = s_levelInfo[i].Key;
			if (DataStore.Instance.LevelsMinTime.ContainsKey(key))
			{
				num += DataStore.Instance.LevelsMinTime[key];
			}
		}
		return TimeSpan.FromSeconds(Mathf.RoundToInt(num));
	}

	public int GetTotalShapes(int minLevel, int maxLevel)
	{
		int num = 0;
		for (int i = minLevel; i <= maxLevel; i++)
		{
			string key = s_levelInfo[i].Key;
			if (DataStore.Instance.LevelsMinShapeCount.ContainsKey(key))
			{
				num += DataStore.Instance.LevelsMinShapeCount[key];
			}
		}
		return num;
	}

	public int GetTotalLevelCount()
	{
		return s_levelInfo.Count;
	}

	public int GetTotalStarsCount()
	{
		int num = 0;
		foreach (int value in DataStore.Instance.LevelsSolved.Values)
		{
			num += getStarCount((LevelCompletion)value);
		}
		return num;
	}

	private int getStarCount(LevelCompletion completion)
	{
		int num = 0;
		if ((completion & LevelCompletion.Solved) == LevelCompletion.Solved)
		{
			num++;
		}
		if ((completion & LevelCompletion.TimeSolved) == LevelCompletion.TimeSolved)
		{
			num++;
		}
		if ((completion & LevelCompletion.ShapeSolved) == LevelCompletion.ShapeSolved)
		{
			num++;
		}
		return num;
	}

	public int GetThreeStarCount()
	{
		int num = 0;
		foreach (int value in DataStore.Instance.LevelsSolved.Values)
		{
			LevelCompletion levelCompletion = (LevelCompletion)value;
			if (levelCompletion == LevelCompletion.Complete)
			{
				num++;
			}
		}
		return num;
	}

	public int GetBelowShapeGoalCount()
	{
		int num = 0;
		foreach (KeyValuePair<string, int> item in DataStore.Instance.LevelsMinShapeCount)
		{
			string key = item.Key;
			int value = item.Value;
			int levelIDFromKey = GetLevelIDFromKey(key);
			if (levelIDFromKey != -1)
			{
				int shapeGoal = s_levelInfo[levelIDFromKey].ShapeGoal;
				if (value < shapeGoal)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetOneShapeCount()
	{
		int num = 0;
		foreach (int value in DataStore.Instance.LevelsMinShapeCount.Values)
		{
			if (value == 1)
			{
				num++;
			}
		}
		return num;
	}

	public int GetLessThanTime(float time)
	{
		int num = 0;
		foreach (float value in DataStore.Instance.LevelsMinTime.Values)
		{
			if (value <= time)
			{
				num++;
			}
		}
		return num;
	}

	public int GetBelowTimeGoalByAmount(float time)
	{
		int num = 0;
		foreach (KeyValuePair<string, float> item in DataStore.Instance.LevelsMinTime)
		{
			string key = item.Key;
			int levelIDFromKey = GetLevelIDFromKey(key);
			float value = item.Value;
			float timeGoal = s_levelInfo[levelIDFromKey].TimeGoal;
			if (timeGoal - value >= time)
			{
				num++;
			}
		}
		return num;
	}

	public bool LoadNextLevel(bool nextUnsolved)
	{
		if (CommunityLevel)
		{
			return CommunityManager.LoadNextLevel();
		}
		if (Level == GetLevelCount())
		{
			AnalyticsManager.LogGameEvent("EndOfGame", "NextPressed");
			return false;
		}
		int level = Level;
		int i = Level + 1;
		if (nextUnsolved)
		{
			for (; i < GetLevelCount() && DataStore.Instance.LevelsSolved.ContainsKey(getLevelInfo(i).Key); i++)
			{
			}
		}
		if (!StoreManager.Instance.IsGameOwned())
		{
			foreach (LevelPack s_levelPack in s_levelPacks)
			{
				if (i >= s_levelPack.FirstLevel && i <= s_levelPack.LastLevel && !s_levelPack.Free && !StoreManager.Instance.CheckIfOwned(s_levelPack.Item))
				{
					Level = s_levelPack.FirstLevel;
					return false;
				}
			}
		}
		if (GetLevelLocked(i))
		{
			return false;
		}
		int levelPackIndex = GetLevelPackIndex(i);
		if (levelPackIndex != -1 && !s_levelPacks[levelPackIndex].Available)
		{
			return false;
		}
		if (i > GetLevelCount())
		{
			Level = level;
			return false;
		}
		Level = i;
		LoadLevel(Level);
		return true;
	}

	public static void LoadLevel(string sceneFile)
	{
		IEnumerable<KeyValuePair<int, LevelInfo>> source = s_levelInfo.Where((KeyValuePair<int, LevelInfo> l) => l.Value.File.Equals(sceneFile));
		if (source.Count() > 0)
		{
			Level = source.ElementAt(0).Key;
			LoadLevel(Level);
		}
	}

	public static void LoadLevel(int level, bool tryLevel = false)
	{
		Level = level;
		s_tryLevel = tryLevel;
		string levelFile = GetLevelFile(Level);
		Instance.StartCoroutine(Instance.LoadLevelCoroutine(levelFile));
	}

	private IEnumerator LoadLevelCoroutine(string levelFile)
	{
		if (bundleLoadRequest != null)
		{
			while (!bundleLoadRequest.isDone)
			{
				yield return 0;
			}
		}
		Debug.Log("Loading static level...");
		SceneManager.LoadSceneAsync("PhysicsGame");
		if (!Application.CanStreamedLevelBeLoaded(levelFile) && m_bundleLevels != null)
		{
			UnityEngine.Object obj = m_bundleLevels.LoadAsset(levelFile);
			if (obj != null)
			{
				Debug.Log("Loaded scene asset from bundle.");
			}
		}
		if (Application.CanStreamedLevelBeLoaded(levelFile))
		{
			SceneManager.LoadScene(levelFile, LoadSceneMode.Additive);
			Debug.Log("Loaded scene.");
		}
		m_levelLoaded = 2;
	}

	public void RetryLevel()
	{
		if (TouchDrawEditor.Instance != null)
		{
			SceneManager.LoadScene("LevelEditor");
			m_levelLoaded = 2;
		}
		else
		{
			LoadLevel(Level, IsTryLevel);
		}
	}

	private static LevelInfo getLevelInfo(int level)
	{
		return s_levelInfo[level];
	}

	private static IEnumerable<LevelInfo> getLevelInfos()
	{
		return s_levelInfo.Values;
	}
}
