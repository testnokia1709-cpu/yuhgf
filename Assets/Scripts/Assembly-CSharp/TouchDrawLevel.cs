using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TouchDrawLevel : ISerializationCallbackReceiver
{
	[NonSerialized]
	public StringId Goal;

	[NonSerialized]
	public LevelPenType PenMaterial;

	[NonSerialized]
	public int Version;

	[NonSerialized]
	public string Name = "Untitled";

	[NonSerialized]
	public string PenMaterialName;

	[NonSerialized]
	public string GoalName;

	[NonSerialized]
	public int ShapeGoal;

	[NonSerialized]
	public float TimeGoal;

	[NonSerialized]
	public List<TouchDrawObject> ObjectList = new List<TouchDrawObject>();

	[NonSerialized]
	public bool ActiveOnStart = true;

	[NonSerialized]
	public bool Ground = true;

	[NonSerialized]
	public List<Vector2> Hint = new List<Vector2>();

	[SerializeField]
	private int v;

	[SerializeField]
	private string n;

	[SerializeField]
	private string p;

	[SerializeField]
	private string g;

	[SerializeField]
	private int s;

	[SerializeField]
	private int t;

	[SerializeField]
	private List<TouchDrawObject> l = new List<TouchDrawObject>();

	[SerializeField]
	private int a;

	[SerializeField]
	private int d = 1;

	[SerializeField]
	private List<int> h;

	public static float s_gridSize = 2.5f;

	private static float s_invGridSize = 1f / s_gridSize;

	public void OnBeforeSerialize()
	{
		v = Version;
		n = Name;
		p = PenMaterialName;
		g = GoalName;
		s = ShapeGoal;
		t = Mathf.RoundToInt(TimeGoal * 10f);
		l = ObjectList;
		a = (ActiveOnStart ? 1 : 0);
		d = (Ground ? 1 : 0);
		h = new List<int>();
		foreach (Vector2 item in Hint)
		{
			h.Add(Mathf.RoundToInt(item.x * 10f));
			h.Add(Mathf.RoundToInt(item.y * 10f));
		}
	}

	public void OnAfterDeserialize()
	{
		Version = v;
		Name = n;
		PenMaterialName = p;
		GoalName = g;
		ShapeGoal = s;
		TimeGoal = (float)t / 10f;
		ObjectList = l;
		ActiveOnStart = a == 1;
		Ground = d == 1;
		Hint = new List<Vector2>();
		for (int i = 0; i < h.Count; i += 2)
		{
			float x = (float)h[i] / 10f;
			float y = (float)h[i + 1] / 10f;
			Hint.Add(new Vector2(x, y));
		}
	}

	public static TouchDrawLevel LoadLevelFromJson(string jsonData)
	{
		TouchDrawLevel touchDrawLevel = null;
		if (jsonData != null)
		{
			try
			{
				touchDrawLevel = JsonUtility.FromJson<TouchDrawLevel>(jsonData.Trim());
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading Json (" + ex.Message + "): " + jsonData);
				touchDrawLevel = null;
			}
		}
		if (touchDrawLevel != null)
		{
			try
			{
				touchDrawLevel.PenMaterial = (LevelPenType)Enum.Parse(typeof(LevelPenType), touchDrawLevel.PenMaterialName);
			}
			catch (Exception)
			{
				touchDrawLevel.PenMaterial = LevelPenType.Normal;
				Debug.LogError("Unknown Pen Material Name: " + touchDrawLevel.PenMaterialName);
			}
			TouchDrawPhysics.Instance.SetShapeMaterial(touchDrawLevel.PenMaterial);
			try
			{
				touchDrawLevel.Goal = (StringId)Enum.Parse(typeof(StringId), touchDrawLevel.GoalName);
			}
			catch (Exception)
			{
				touchDrawLevel.Goal = StringId.S_EMPTY;
				Debug.LogError("Unknown Goal Name: " + touchDrawLevel.GoalName);
			}
		}
		else
		{
			touchDrawLevel = CreateLevel();
		}
		return touchDrawLevel;
	}

	public static string SaveLevelToJson(TouchDrawLevel level)
	{
		level.TimeGoal = (float)Math.Round(level.TimeGoal, 2);
		return JsonUtility.ToJson(level);
	}

	public void ResetGoal()
	{
		TouchDrawPhysics.Instance.Ground.SetActive(true);
		ContactTrigger component = TouchDrawPhysics.Instance.Ground.GetComponent<ContactTrigger>();
		ContactTrigger component2 = TouchDrawPhysics.Instance.WallLeft.GetComponent<ContactTrigger>();
		ContactTrigger component3 = TouchDrawPhysics.Instance.WallRight.GetComponent<ContactTrigger>();
		ContactTrigger component4 = TouchDrawPhysics.Instance.Ceiling.GetComponent<ContactTrigger>();
		component.Reset();
		component2.Reset();
		component3.Reset();
		component4.Reset();
		foreach (TouchDrawObject item in getObjectsByType(LevelObjectType.ORANGEBOX))
		{
			TimedZoneTrigger componentInChildren = item.Object.GetComponentInChildren<TimedZoneTrigger>();
			componentInChildren.enabled = false;
		}
	}

	public bool SetupGoal()
	{
		ResetGoal();
		ContactTrigger component = TouchDrawPhysics.Instance.Ground.GetComponent<ContactTrigger>();
		ContactTrigger component2 = TouchDrawPhysics.Instance.WallLeft.GetComponent<ContactTrigger>();
		ContactTrigger component3 = TouchDrawPhysics.Instance.WallRight.GetComponent<ContactTrigger>();
		ContactTrigger component4 = TouchDrawPhysics.Instance.Ceiling.GetComponent<ContactTrigger>();
		TouchDrawPhysics.Instance.Ground.SetActive(Ground);
		if (Goal == StringId.S_BALL_TOUCH_GROUND)
		{
			List<TouchDrawObject> objectsByType = getObjectsByType(LevelObjectType.ORANGEBALL);
			if (objectsByType.Count == 1)
			{
				component.SetFilterObject(objectsByType[0].Object);
				component.enabled = true;
				component.ContactCount = 1;
				return true;
			}
			DialogManager.ShowDialog("Warning: You must have one and only one orange ball in the level for this goal.", TextLibrary.Get(StringId.S_OK));
		}
		else if (Goal == StringId.S_BALL_TOUCH_LEFTWALL)
		{
			List<TouchDrawObject> objectsByType2 = getObjectsByType(LevelObjectType.ORANGEBALL);
			if (objectsByType2.Count == 1)
			{
				component2.SetFilterObject(objectsByType2[0].Object);
				component2.enabled = true;
				component2.ContactCount = 1;
				return true;
			}
			DialogManager.ShowDialog("Warning: You must have one and only one orange ball in the level for this goal.", TextLibrary.Get(StringId.S_OK));
		}
		else if (Goal == StringId.S_BALL_TOUCH_RIGHTWALL)
		{
			List<TouchDrawObject> objectsByType3 = getObjectsByType(LevelObjectType.ORANGEBALL);
			if (objectsByType3.Count == 1)
			{
				component3.SetFilterObject(objectsByType3[0].Object);
				component3.enabled = true;
				component3.ContactCount = 1;
				return true;
			}
			DialogManager.ShowDialog("Warning: You must have one and only one orange ball in the level for this goal.", TextLibrary.Get(StringId.S_OK));
		}
		else if (Goal == StringId.S_BALL_TOUCH_CEILING)
		{
			List<TouchDrawObject> objectsByType4 = getObjectsByType(LevelObjectType.ORANGEBALL);
			if (objectsByType4.Count == 1)
			{
				component4.SetFilterObject(objectsByType4[0].Object);
				component4.enabled = true;
				component4.ContactCount = 1;
				return true;
			}
			DialogManager.ShowDialog("Warning: You must have one and only one orange ball in the level for this goal.", TextLibrary.Get(StringId.S_OK));
		}
		else
		{
			if (Goal == StringId.S_BALL_INSIDE_ORANGE_BOX)
			{
				List<TouchDrawObject> objectsByType5 = getObjectsByType(LevelObjectType.ORANGEBALL);
				if (objectsByType5.Count != 1)
				{
					DialogManager.ShowDialog("Warning: You must have one orange ball in the level for this goal.", TextLibrary.Get(StringId.S_OK));
					return false;
				}
				TouchDrawObject touchDrawObject = objectsByType5[0];
				List<TouchDrawObject> objectsByType6 = getObjectsByType(LevelObjectType.ORANGEBOX);
				if (objectsByType6.Count != 1)
				{
					DialogManager.ShowDialog("Warning: You must have one orange box in the level for this goal.", TextLibrary.Get(StringId.S_OK));
					return false;
				}
				TouchDrawObject touchDrawObject2 = objectsByType6[0];
				TimedZoneTrigger componentInChildren = touchDrawObject2.Object.GetComponentInChildren<TimedZoneTrigger>();
				componentInChildren.enabled = true;
				componentInChildren.SetFilterObject(touchDrawObject.Object);
				return true;
			}
			if (Goal == StringId.S_OBJECT_INSIDE_ORANGE_BOX)
			{
				List<TouchDrawObject> objectsByType7 = getObjectsByType(LevelObjectType.ORANGEBOX);
				if (objectsByType7.Count != 1)
				{
					DialogManager.ShowDialog("Warning: You must have one orange box in the level for this goal.", TextLibrary.Get(StringId.S_OK));
					return false;
				}
				TouchDrawObject touchDrawObject3 = objectsByType7[0];
				TimedZoneTrigger componentInChildren2 = touchDrawObject3.Object.GetComponentInChildren<TimedZoneTrigger>();
				componentInChildren2.enabled = true;
				componentInChildren2.GoalCount = 1;
				componentInChildren2.SetGreaterThanOrEqual();
				componentInChildren2.SetFilterObject(null);
				return true;
			}
			if (Goal == StringId.S_BALL_TOUCH_LEFTRIGHT)
			{
				List<TouchDrawObject> objectsByType8 = getObjectsByType(LevelObjectType.ORANGEBALL);
				if (objectsByType8.Count != 1)
				{
					DialogManager.ShowDialog("Warning: You must have one orange ball in the level for this goal.", TextLibrary.Get(StringId.S_OK));
					return false;
				}
				TouchDrawObject touchDrawObject4 = objectsByType8[0];
				component2.SetFilterObject(touchDrawObject4.Object);
				component2.enabled = true;
				component2.ContactCount = 1;
				component2.DependentTriggers.Add(component3);
				component3.SetFilterObject(objectsByType8[0].Object);
				component3.enabled = true;
				component3.ContactCount = 1;
				component3.DependentTriggers.Add(component2);
				return true;
			}
			DialogManager.ShowDialog("Warning: Please select a valid level goal.", TextLibrary.Get(StringId.S_OK));
		}
		return false;
	}

	public static int GetObjectIndex(string objType)
	{
		int result = -1;
		foreach (GameObject objectPrefab in TouchDrawDefinition.Instance.ObjectPrefabs)
		{
			if (objectPrefab.name == objType)
			{
				result = TouchDrawDefinition.Instance.ObjectPrefabs.IndexOf(objectPrefab);
				break;
			}
		}
		return result;
	}

	public static void RestoreLevel(TouchDrawLevel level, Vector2 offset)
	{
		List<TouchDrawObject> list = null;
		foreach (TouchDrawObject @object in level.ObjectList)
		{
			UnityEngine.Object.DestroyImmediate(@object.Object);
			int objectIndex = GetObjectIndex(@object.ObjectName);
			if (objectIndex != -1)
			{
				RestoreObject(@object, objectIndex, @object.Position + offset);
				continue;
			}
			Debug.LogError("Invalid Object requested: " + @object.ObjectName);
			if (list == null)
			{
				list = new List<TouchDrawObject>();
			}
			list.Add(@object);
		}
		if (list == null)
		{
			return;
		}
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_LEVEL_ERROR), TextLibrary.Get(StringId.S_OK));
		foreach (TouchDrawObject item in list)
		{
			level.ObjectList.Remove(item);
		}
	}

	public static void RestoreObject(TouchDrawObject tdObj, int index, Vector3 position)
	{
		if (index > -1 && index < TouchDrawDefinition.Instance.ObjectPrefabs.Count)
		{
			GameObject gameObject = TouchDrawDefinition.Instance.ObjectPrefabs[index];
			Vector3 position2 = new Vector3(position.x, position.y, gameObject.transform.position.z);
			GameObject gameObject2 = (tdObj.Object = UnityEngine.Object.Instantiate(gameObject, position2, Quaternion.identity));
			tdObj.Collider = gameObject2.GetComponentInChildren<Collider2D>();
			tdObj.ObjectName = gameObject.name;
			tdObj.LevelObject = gameObject2.GetComponent<LevelObject>();
			tdObj.LevelObject.ShowOutline(false);
			tdObj.Object.transform.parent = TouchDrawPhysics.Instance.ObjectsParent.transform;
			tdObj.Position = RoundToGrid(position);
			tdObj.Object.transform.Rotate(Vector3.back, tdObj.Rotation);
			customizeObject(tdObj);
		}
		else
		{
			Debug.LogError("Invalid Object requested: " + tdObj.ObjectName);
		}
	}

	public static TouchDrawObject AddObject(TouchDrawLevel level, int index, Vector3 position)
	{
		Vector3 position2 = new Vector3(position.x, position.y, TouchDrawDefinition.Instance.ObjectPrefabs[index].transform.position.z);
		GameObject gameObject = UnityEngine.Object.Instantiate(TouchDrawDefinition.Instance.ObjectPrefabs[index], position2, Quaternion.identity);
		TouchDrawObject touchDrawObject = new TouchDrawObject();
		touchDrawObject.Object = gameObject;
		touchDrawObject.Collider = gameObject.GetComponentInChildren<Collider2D>();
		touchDrawObject.ObjectName = TouchDrawDefinition.Instance.ObjectPrefabs[index].name;
		touchDrawObject.LevelObject = gameObject.GetComponent<LevelObject>();
		touchDrawObject.LevelObject.ShowOutline(false);
		touchDrawObject.Object.transform.parent = TouchDrawPhysics.Instance.ObjectsParent.transform;
		touchDrawObject.Position = RoundToGrid(position);
		touchDrawObject.ZPos = position2.z;
		customizeObject(touchDrawObject);
		level.ObjectList.Add(touchDrawObject);
		return touchDrawObject;
	}

	public static TouchDrawObject CloneObject(TouchDrawLevel level, TouchDrawObject tdObj, Vector3 position)
	{
		Vector2 position2 = RoundToGrid(position);
		Vector3 position3 = new Vector3(position2.x, position2.y, getObjectZDepth(tdObj));
		GameObject gameObject = UnityEngine.Object.Instantiate(tdObj.Object, position3, tdObj.Object.transform.rotation);
		TouchDrawObject touchDrawObject = new TouchDrawObject();
		touchDrawObject.Object = gameObject;
		touchDrawObject.Collider = gameObject.GetComponentInChildren<Collider2D>();
		touchDrawObject.ObjectName = tdObj.ObjectName;
		touchDrawObject.LevelObject = gameObject.GetComponent<LevelObject>();
		touchDrawObject.LevelObject.ShowOutline(false);
		touchDrawObject.Object.transform.parent = TouchDrawPhysics.Instance.ObjectsParent.transform;
		touchDrawObject.Object.transform.localScale = tdObj.Object.transform.localScale;
		touchDrawObject.Position = position2;
		touchDrawObject.Rotation = tdObj.Rotation;
		touchDrawObject.ZPos = position3.z;
		customizeObject(touchDrawObject);
		level.ObjectList.Add(touchDrawObject);
		return touchDrawObject;
	}

	private static float getObjectZDepth(TouchDrawObject tdObj)
	{
		float result = 0f;
		foreach (GameObject objectPrefab in TouchDrawDefinition.Instance.ObjectPrefabs)
		{
			if (objectPrefab.name.Equals(tdObj.ObjectName))
			{
				result = objectPrefab.transform.position.z;
				break;
			}
		}
		return result;
	}

	public void RemoveObject(TouchDrawObject tdObj)
	{
		if (tdObj.Object != null)
		{
			UnityEngine.Object.Destroy(tdObj.Object);
		}
		ObjectList.Remove(tdObj);
	}

	public static void ClearObjects(TouchDrawLevel level)
	{
		foreach (TouchDrawObject @object in level.ObjectList)
		{
			UnityEngine.Object.DestroyImmediate(@object.Object);
		}
	}

	public static TouchDrawLevel CreateLevel()
	{
		TouchDrawLevel touchDrawLevel = new TouchDrawLevel();
		touchDrawLevel.Goal = StringId.S_BALL_TOUCH_GROUND;
		touchDrawLevel.PenMaterial = LevelPenType.Normal;
		touchDrawLevel.GoalName = Enum.GetName(typeof(StringId), touchDrawLevel.Goal);
		touchDrawLevel.PenMaterialName = touchDrawLevel.PenMaterial.ToString();
		return touchDrawLevel;
	}

	public static Vector2 RoundToGrid(Vector2 worldPos)
	{
		return new Vector2(Mathf.Round(worldPos.x * s_invGridSize) * s_gridSize, Mathf.Round(worldPos.y * s_invGridSize) * s_gridSize);
	}

	public static string EncodeLevel(TouchDrawLevel level)
	{
		string text = null;
		try
		{
			string text2 = SaveLevelToJson(level);
			Debug.Log("json: " + text2);
			byte[] array = Compress.CompressText(text2);
			text = Convert.ToBase64String(array, Base64FormattingOptions.None);
			Debug.Log("Compressed: " + array.Length + " Base: " + text2.Length + " Encoded: " + text.Length);
		}
		catch (Exception)
		{
		}
		return text;
	}

	public static TouchDrawLevel DecodeLevel(string levelData)
	{
		TouchDrawLevel result = null;
		try
		{
			if (!string.IsNullOrEmpty(levelData))
			{
				levelData = levelData.Replace(" ", "+").Trim();
				int num = levelData.Length % 4;
				if (num != 0)
				{
					levelData = levelData.PadRight(levelData.Length + (4 - num), '=');
				}
				byte[] byteData = Convert.FromBase64String(levelData);
				string jsonData = Compress.DecompressText(byteData);
				result = LoadLevelFromJson(jsonData);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
		return result;
	}

	public void StoreHint(TouchDrawRecorder recorder)
	{
		Hint = new List<Vector2>();
		Vector2 vector = new Vector2(1000f, 1000f);
		foreach (TimedPoint point in recorder.Points)
		{
			if (point.PointType == PointType.Begin || point.PointType == PointType.Middle)
			{
				Hint.Add(point.Point);
			}
			else if (point.PointType == PointType.End)
			{
				if (point.Point != vector)
				{
					Hint.Add(point.Point);
				}
				Hint.Add(point.Point);
			}
			vector = point.Point;
		}
	}

	private static void customizeObject(TouchDrawObject tdObj)
	{
		if (tdObj.LevelObject.Type == LevelObjectType.NODRAW)
		{
			tdObj.Object.transform.position += new Vector3(0f, 0f, 1f);
		}
	}

	private List<TouchDrawObject> getObjectsByType(LevelObjectType type)
	{
		return ObjectList.Where((TouchDrawObject o) => o.LevelObject.Type == type).ToList();
	}
}
