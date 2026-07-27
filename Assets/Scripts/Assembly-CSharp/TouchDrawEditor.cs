using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TouchDrawEditor : MonoBehaviour
{
	public static TouchDrawEditor Instance;

	[HideInInspector]
	public UIBase UserInterface;

	public GameObject MoveControl;

	public GameObject ObjectPickerContainer;

	public GameObject ObjectItemTemplate;

	public GameObject GoalPickerContainer;

	public GameObject GoalItemTemplate;

	public GameObject MultiSelectBox;

	public StringId SelectedGoal;

	public Sprite SelectedObjectSprite;

	public int SelectedObjectIndex;

	public float SelectedObjectRotation;

	public EditorMode Mode;

	public float RotationIncrement = 90f;

	public List<GameObject> IgnoreObjectPrefabs;

	[HideInInspector]
	public int ShapeGoal;

	[HideInInspector]
	public float TimeGoal;

	[HideInInspector]
	public TouchDrawLevel Level = new TouchDrawLevel();

	[HideInInspector]
	public bool TouchEnabled = true;

	public static Vector2 ClampBottomLeft = new Vector2(-48.5f, -48.5f);

	public static Vector2 ClampTopRight = new Vector2(48.5f, 48.5f);

	private static int s_maxMoveable = 20;

	private bool m_badInput;

	private Vector2 m_mouseDownPosition;

	private Vector2 m_mouseDownObjPosition;

	private GameObject m_mouseDownObj;

	private List<TouchDrawObject> m_multiSelectList = new List<TouchDrawObject>();

	private List<Vector2> m_multiSelectPosition = new List<Vector2>();

	private Vector2 m_multiSelectStart;

	private Vector2 m_multiSelectEnd;

	private TouchDrawObject m_selectedObject;

	public bool IsObjectSelected
	{
		get
		{
			return m_mouseDownObj != null;
		}
	}

	public bool IsMultiObjectSelected
	{
		get
		{
			return m_multiSelectList.Count > 0;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		GridLayoutGroup component = ObjectPickerContainer.GetComponent<GridLayoutGroup>();
		float num = Screen.height / 10;
		component.cellSize = new Vector2(num, num);
		foreach (GameObject objectPrefab in TouchDrawDefinition.Instance.ObjectPrefabs)
		{
			if (IgnoreObjectPrefabs.Contains(objectPrefab))
			{
				continue;
			}
			Sprite sprite = objectPrefab.GetComponent<LevelObject>().Sprite;
			GameObject gameObject = UnityEngine.Object.Instantiate(ObjectItemTemplate);
			gameObject.transform.parent = ObjectPickerContainer.transform;
			ObjectPickerButtonTemplate component2 = gameObject.GetComponent<ObjectPickerButtonTemplate>();
			component2.ObjectImage.sprite = sprite;
			int index = TouchDrawDefinition.Instance.ObjectPrefabs.IndexOf(objectPrefab);
			Button component3 = gameObject.GetComponent<Button>();
			component3.onClick.AddListener(delegate
			{
				if (addObject(index, Vector3.zero) != null)
				{
					UserInterface.OnObjectAdded();
				}
			});
			gameObject.SetActive(true);
		}
		foreach (StringId goalString in TouchDrawDefinition.Instance.GoalStrings)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(GoalItemTemplate);
			gameObject2.transform.parent = GoalPickerContainer.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			Text componentInChildren = gameObject2.GetComponentInChildren<Text>();
			componentInChildren.text = TextLibrary.Get(goalString);
			Button component4 = gameObject2.GetComponent<Button>();
			StringId goalId = goalString;
			component4.onClick.AddListener(delegate
			{
				setGoal(goalId);
			});
			gameObject2.SetActive(true);
		}
		Mode = EditorMode.EditObject;
		MoveControl.SetActive(false);
		MultiSelectBox.SetActive(false);
		SelectedObjectIndex = 0;
		SelectedObjectRotation = 0f;
		UserInterface.OnObjectAdded();
		TouchDrawPhysics.Instance.SetBoundary(ClampBottomLeft, ClampTopRight);
	}

	private void Update()
	{
		if (!TouchEnabled || DialogManager.Instance.IsShown)
		{
			return;
		}
		bool flag = Input.GetMouseButtonDown(0);
		bool flag2 = Input.GetMouseButtonUp(0);
		bool flag3 = Input.GetMouseButton(0);
		Vector2 vector = Input.mousePosition;
		if (Input.touches.Length == 0 && m_badInput)
		{
			m_badInput = false;
		}
		if (Input.touches.Length == 1 && !m_badInput)
		{
			Touch touch = Input.touches[0];
			flag = touch.phase == TouchPhase.Began;
			flag2 = touch.phase == TouchPhase.Ended;
			flag3 = touch.phase == TouchPhase.Moved;
			vector = touch.position;
		}
		if (Input.touches.Length > 1)
		{
			flag2 = !m_badInput;
			m_badInput = true;
			flag = false;
			flag3 = false;
			vector = Input.touches[0].position;
		}
		Vector2 vector2 = Camera.main.ScreenToWorldPoint(vector);
		bool flag4 = isValidPosition(vector2);
		if (TouchDrawPhysics.Instance.IsPhysicsEnabled)
		{
			return;
		}
		bool flag5 = false;
		bool flag6 = false;
		float num = 1000f;
		TouchDrawObject selection = null;
		if (flag && flag4)
		{
			foreach (TouchDrawObject @object in Level.ObjectList)
			{
				Bounds bounds = @object.Collider.bounds;
				bounds.center = new Vector3(bounds.center.x, bounds.center.y, 0f);
				if (bounds.Contains(vector2) && @object.Object.transform.position.z < num)
				{
					flag5 = true;
					selection = @object;
					m_mouseDownObj = @object.Object;
					m_mouseDownObjPosition = @object.Object.transform.position;
					m_mouseDownPosition = vector2;
					if (!IsMultiObjectSelected)
					{
						num = @object.Object.transform.position.z;
						SetSelection(@object);
					}
					else if (IsMultiObjectSelected && m_multiSelectList.Contains(@object))
					{
						flag6 = true;
					}
				}
			}
			if (!flag5)
			{
				ClearSelection();
			}
			else if (flag5 && IsMultiObjectSelected && !flag6)
			{
				SetSelection(selection);
			}
			if (Mode == EditorMode.AddObject && !flag5)
			{
				addObject(SelectedObjectIndex, vector2, SelectedObjectRotation);
			}
			else if (Mode == EditorMode.EditObject && !flag5)
			{
				ClearSelection();
				m_multiSelectStart = vector2;
				m_multiSelectEnd = vector2;
				MultiSelectBox.SetActive(true);
				updateMultiSelectBox();
			}
			if (IsMultiObjectSelected)
			{
				updateMultiSelectPositions();
			}
		}
		if (flag2)
		{
			MultiSelectBox.SetActive(false);
			if (!m_multiSelectStart.Equals(m_multiSelectEnd))
			{
				SetSelection(getObjectsWithSelectBox(m_multiSelectStart, m_multiSelectEnd));
			}
			m_multiSelectEnd = m_multiSelectStart;
		}
		if (!flag3 || !flag4)
		{
			return;
		}
		if (m_mouseDownObj != null)
		{
			TouchDrawObject touchDrawObject = GetObject(m_mouseDownObj);
			if (m_multiSelectList.Contains(touchDrawObject))
			{
				Vector2 zero = Vector2.zero;
				int num2 = 0;
				int num3 = 0;
				bool flag7 = false;
				foreach (TouchDrawObject multiSelect in m_multiSelectList)
				{
					Vector2 adjustment = Vector2.zero;
					Vector2 vector3 = m_multiSelectPosition[num3];
					Vector2 vector4 = TouchDrawLevel.RoundToGrid(m_multiSelectPosition[num3] + (vector2 - m_mouseDownPosition));
					if (vector3 != vector4)
					{
						multiSelect.Object.transform.position = new Vector3(vector4.x, vector4.y, multiSelect.Object.transform.position.z);
						multiSelect.Position = multiSelect.Object.transform.position;
						num2++;
					}
					flag7 |= isOutOfBounds(multiSelect, ref adjustment);
					if (Math.Abs(adjustment.y) > Math.Abs(zero.y))
					{
						zero.y = adjustment.y;
					}
					if (Math.Abs(adjustment.x) > Math.Abs(zero.x))
					{
						zero.x = adjustment.x;
					}
					num3++;
				}
				if (flag7)
				{
					foreach (TouchDrawObject multiSelect2 in m_multiSelectList)
					{
						Vector3 position = multiSelect2.Object.transform.position;
						multiSelect2.Object.transform.position += (Vector3)zero;
					}
					return;
				}
				if (num2 == m_multiSelectList.Count)
				{
					resetTimeShapeGoals();
				}
			}
			else
			{
				Vector3 position2 = m_mouseDownObj.transform.position;
				Vector2 vector5 = TouchDrawLevel.RoundToGrid(m_mouseDownObjPosition + (vector2 - m_mouseDownPosition));
				m_mouseDownObj.transform.position = new Vector3(vector5.x, vector5.y, m_mouseDownObj.transform.position.z);
				isOutOfBounds(touchDrawObject, true);
				Vector3 vector6 = m_mouseDownObj.transform.TransformPoint(touchDrawObject.LevelObject.Offset);
				MoveControl.transform.position = new Vector3(vector6.x, vector6.y, MoveControl.transform.position.z);
				if (position2 != m_mouseDownObj.transform.position)
				{
					resetTimeShapeGoals();
				}
			}
		}
		else
		{
			m_multiSelectEnd = vector2;
			updateMultiSelectBox();
		}
	}

	public void SaveLevel()
	{
		foreach (TouchDrawObject @object in Level.ObjectList)
		{
			@object.Position = @object.Object.transform.position;
		}
		Level.PenMaterialName = Level.PenMaterial.ToString();
		Level.GoalName = Enum.GetName(typeof(StringId), Level.Goal);
	}

	public void LoadLevel(TouchDrawLevel level)
	{
		ClearLevel();
		if (level != null)
		{
			Level = level;
			RestoreLevel();
		}
	}

	public void ClearLevel()
	{
		ClearSelection();
		TouchDrawLevel.ClearObjects(Level);
		Level = TouchDrawLevel.CreateLevel();
		UserInterface.UpdateUI(Level.TimeGoal, Level.ShapeGoal);
	}

	public void RestoreLevel()
	{
		TouchDrawLevel.RestoreLevel(Level, Vector2.zero);
		UserInterface.UpdateUI(Level.TimeGoal, Level.ShapeGoal);
	}

	public void DeleteObject()
	{
		if (m_multiSelectList.Count > 0)
		{
			foreach (TouchDrawObject multiSelect in m_multiSelectList)
			{
				Level.RemoveObject(multiSelect);
			}
		}
		else if (m_selectedObject != null)
		{
			Level.RemoveObject(m_selectedObject);
			resetTimeShapeGoals();
		}
		m_selectedObject = null;
		ClearSelection();
	}

	public void CloneObject()
	{
		float num = TouchDrawLevel.s_gridSize * 2f;
		if (m_multiSelectList.Count > 0)
		{
			List<TouchDrawObject> list = new List<TouchDrawObject>();
			foreach (TouchDrawObject multiSelect in m_multiSelectList)
			{
				TouchDrawObject touchDrawObject = cloneObject(multiSelect, multiSelect.Position + new Vector2(num, 0f - num));
				if (touchDrawObject != null)
				{
					isOutOfBounds(touchDrawObject, true);
					list.Add(touchDrawObject);
				}
			}
			SetSelection(list);
		}
		else if (m_mouseDownObj != null)
		{
			TouchDrawObject touchDrawObject2 = GetObject(m_mouseDownObj);
			TouchDrawObject touchDrawObject3 = cloneObject(touchDrawObject2, touchDrawObject2.Position + new Vector2(num, 0f - num));
			if (touchDrawObject3 != null)
			{
				isOutOfBounds(touchDrawObject3, true);
				SetSelection(touchDrawObject3);
			}
		}
	}

	public void RotateObject()
	{
		if (!(m_mouseDownObj != null))
		{
			return;
		}
		foreach (TouchDrawObject @object in Level.ObjectList)
		{
			if (@object.Object == m_mouseDownObj)
			{
				@object.Rotation += RotationIncrement;
				if (@object.Rotation >= 360f)
				{
					@object.Rotation = 0f;
				}
				@object.Object.transform.Rotate(Vector3.back, RotationIncrement);
				isOutOfBounds(@object, true);
				SetSelection(@object);
				resetTimeShapeGoals();
				break;
			}
		}
	}

	public bool ContainsObject(GameObject obj)
	{
		foreach (TouchDrawObject @object in Level.ObjectList)
		{
			if (@object.Object == obj)
			{
				return true;
			}
		}
		return false;
	}

	public TouchDrawObject GetObject(GameObject obj)
	{
		foreach (TouchDrawObject @object in Level.ObjectList)
		{
			if (@object.Object == obj)
			{
				return @object;
			}
		}
		return null;
	}

	public void ClearSelection()
	{
		if (m_selectedObject != null)
		{
			m_selectedObject.LevelObject.ShowOutline(false);
			if (m_selectedObject.LevelObject.Type != LevelObjectType.NODRAW)
			{
				m_selectedObject.Object.transform.position = new Vector3(m_selectedObject.Object.transform.position.x, m_selectedObject.Object.transform.position.y, m_selectedObject.ZPos);
			}
		}
		m_selectedObject = null;
		foreach (TouchDrawObject multiSelect in m_multiSelectList)
		{
			multiSelect.LevelObject.ShowOutline(false);
			if (multiSelect.LevelObject.Type != LevelObjectType.NODRAW)
			{
				multiSelect.Object.transform.position = new Vector3(multiSelect.Object.transform.position.x, multiSelect.Object.transform.position.y, multiSelect.ZPos);
			}
		}
		m_multiSelectList.Clear();
		MoveControl.SetActive(false);
		m_mouseDownObj = null;
		UserInterface.OnObjectDeselected();
	}

	public void SetSelection(List<TouchDrawObject> objList)
	{
		ClearSelection();
		m_multiSelectList = objList;
		foreach (TouchDrawObject multiSelect in m_multiSelectList)
		{
			multiSelect.LevelObject.ShowOutline(true);
			multiSelect.ZPos = multiSelect.Object.transform.position.z;
			multiSelect.Object.transform.position = new Vector3(multiSelect.Object.transform.position.x, multiSelect.Object.transform.position.y, multiSelect.ZPos - 5f);
		}
		if (m_multiSelectList.Count > 1)
		{
			MoveControl.SetActive(false);
		}
		UserInterface.OnObjectSelected();
	}

	public void SetSelection(TouchDrawObject tdObj)
	{
		ClearSelection();
		MoveControl.SetActive(true);
		Vector3 vector = tdObj.Object.transform.TransformPoint(tdObj.LevelObject.Offset);
		MoveControl.transform.position = new Vector3(vector.x, vector.y, MoveControl.transform.position.z);
		m_mouseDownObj = tdObj.Object;
		m_selectedObject = tdObj;
		SelectedObjectIndex = TouchDrawLevel.GetObjectIndex(tdObj.ObjectName);
		SelectedObjectRotation = tdObj.Rotation;
		tdObj.LevelObject.ShowOutline(true);
		if (tdObj.LevelObject.Type != LevelObjectType.NODRAW)
		{
			tdObj.ZPos = tdObj.Object.transform.position.z;
			tdObj.Object.transform.position = new Vector3(tdObj.Object.transform.position.x, tdObj.Object.transform.position.y, tdObj.ZPos - 5f);
		}
		UserInterface.OnObjectSelected();
	}

	public void SetPen(LevelPenType penType)
	{
		if (Level.PenMaterial != penType)
		{
			Level.PenMaterial = penType;
			TouchDrawPhysics.Instance.SetShapeMaterial(penType);
			resetTimeShapeGoals();
		}
	}

	public void SetActiveOnStart(bool activeOnStart)
	{
		if (Level.ActiveOnStart != activeOnStart)
		{
			Level.ActiveOnStart = activeOnStart;
			resetTimeShapeGoals();
		}
	}

	public void SetGround(bool ground)
	{
		if (Level.Ground != ground)
		{
			Level.Ground = ground;
			resetTimeShapeGoals();
		}
	}

	private TouchDrawObject addObject(int index, Vector3 position, float rotation = 0f)
	{
		TouchDrawObject touchDrawObject = TouchDrawLevel.AddObject(Level, index, position);
		touchDrawObject.Rotation = rotation;
		touchDrawObject.Object.transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.back);
		if (!isObjectAllowed(touchDrawObject))
		{
			Level.RemoveObject(touchDrawObject);
			touchDrawObject = null;
		}
		else
		{
			SetSelection(touchDrawObject);
			resetTimeShapeGoals();
		}
		return touchDrawObject;
	}

	private TouchDrawObject cloneObject(TouchDrawObject tdObj, Vector3 position)
	{
		TouchDrawObject touchDrawObject = TouchDrawLevel.CloneObject(Level, tdObj, position);
		if (!isObjectAllowed(touchDrawObject))
		{
			Level.RemoveObject(touchDrawObject);
			touchDrawObject = null;
		}
		else
		{
			resetTimeShapeGoals();
		}
		return touchDrawObject;
	}

	private void resetTimeShapeGoals()
	{
		Level.TimeGoal = 0f;
		Level.ShapeGoal = 0;
		UserInterface.UpdateUI(0f, 0);
		Level.Hint.Clear();
	}

	private void setGoal(StringId goal)
	{
		if (Level.Goal != goal)
		{
			Level.Goal = goal;
			resetTimeShapeGoals();
			UserInterface.OnGoalSelected();
		}
	}

	private bool isValidPosition(Vector2 position)
	{
		float num = Mathf.Clamp(position.x, ClampBottomLeft.x, ClampTopRight.x);
		float num2 = Mathf.Clamp(position.y, ClampBottomLeft.y, ClampTopRight.y);
		if (Math.Abs(position.x - num) > TouchDrawPhysics.Instance.PointRadius || Math.Abs(position.y - num2) > TouchDrawPhysics.Instance.PointRadius)
		{
			return false;
		}
		return true;
	}

	private bool clampPosition(ref Vector2 position)
	{
		bool result = false;
		float num = Mathf.Clamp(position.x, ClampBottomLeft.x, ClampTopRight.x);
		float num2 = Mathf.Clamp(position.y, ClampBottomLeft.y, ClampTopRight.y);
		if (Math.Abs(position.x - num) > TouchDrawPhysics.Instance.PointRadius || Math.Abs(position.y - num2) > TouchDrawPhysics.Instance.PointRadius)
		{
			result = true;
		}
		position.x = num;
		position.y = num2;
		return result;
	}

	private bool isObjectAllowed(TouchDrawObject tdObj)
	{
		int num = Level.ObjectList.Where((TouchDrawObject o) => o.LevelObject.Type == LevelObjectType.MOVEABLE).Count();
		if (tdObj.LevelObject.Type == LevelObjectType.MOVEABLE && num >= s_maxMoveable)
		{
			return false;
		}
		return true;
	}

	private bool isOutOfBounds(TouchDrawObject tdObj, bool snapToEdge = false)
	{
		Vector2 adjustment = Vector2.zero;
		return isOutOfBounds(tdObj, ref adjustment, snapToEdge);
	}

	private bool isOutOfBounds(TouchDrawObject tdObj, ref Vector2 adjustment, bool snapToEdge = false)
	{
		bool flag = false;
		Collider2D componentInChildren = tdObj.Object.GetComponentInChildren<Collider2D>();
		if (componentInChildren == null)
		{
			return false;
		}
		BoxCollider2D componentInChildren2 = TouchDrawPhysics.Instance.Ground.GetComponentInChildren<BoxCollider2D>();
		BoxCollider2D componentInChildren3 = TouchDrawPhysics.Instance.Ceiling.GetComponentInChildren<BoxCollider2D>();
		BoxCollider2D componentInChildren4 = TouchDrawPhysics.Instance.WallLeft.GetComponentInChildren<BoxCollider2D>();
		BoxCollider2D componentInChildren5 = TouchDrawPhysics.Instance.WallRight.GetComponentInChildren<BoxCollider2D>();
		Bounds bounds = componentInChildren.bounds;
		bounds.center = new Vector3(bounds.center.x, bounds.center.y, 0f);
		if (bounds.Intersects(componentInChildren2.bounds))
		{
			flag = (byte)((flag ? 1u : 0u) | 1u) != 0;
			adjustment.y = componentInChildren2.bounds.center.y + componentInChildren2.bounds.extents.y - (bounds.center.y - bounds.extents.y);
		}
		if (bounds.Intersects(componentInChildren3.bounds))
		{
			flag = (byte)((flag ? 1u : 0u) | 1u) != 0;
			adjustment.y = componentInChildren3.bounds.center.y - componentInChildren3.bounds.extents.y - (bounds.center.y + bounds.extents.y);
		}
		if (bounds.Intersects(componentInChildren4.bounds))
		{
			flag = (byte)((flag ? 1u : 0u) | 1u) != 0;
			adjustment.x = componentInChildren4.bounds.center.x + componentInChildren4.bounds.extents.x - (bounds.center.x - bounds.extents.x);
		}
		if (bounds.Intersects(componentInChildren5.bounds))
		{
			flag = (byte)((flag ? 1u : 0u) | 1u) != 0;
			adjustment.x = componentInChildren5.bounds.center.x - componentInChildren5.bounds.extents.x - (bounds.center.x + bounds.extents.x);
		}
		if (snapToEdge)
		{
			Vector3 position = tdObj.Object.transform.position;
			position = new Vector3(position.x + adjustment.x, position.y + adjustment.y, position.z);
			tdObj.Object.transform.position = position;
			tdObj.Position = position;
		}
		return flag;
	}

	private void updateMultiSelectBox()
	{
		clampPosition(ref m_multiSelectStart);
		clampPosition(ref m_multiSelectEnd);
		Vector2 vector = m_multiSelectEnd - m_multiSelectStart;
		Vector2 vector2 = (m_multiSelectStart + m_multiSelectEnd) / 2f;
		MultiSelectBox.transform.position = new Vector3(vector2.x, vector2.y, MultiSelectBox.transform.position.z);
		MultiSelectBox.transform.localScale = new Vector2(Mathf.Abs(vector.x) / 100f, Mathf.Abs(vector.y) / 100f);
	}

	private List<TouchDrawObject> getObjectsWithSelectBox(Vector2 start, Vector2 end)
	{
		List<TouchDrawObject> list = new List<TouchDrawObject>();
		Vector2 vector = end - start;
		Vector2 vector2 = (start + end) / 2f;
		Bounds bounds = new Bounds(vector2, new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), 100f));
		foreach (TouchDrawObject @object in Level.ObjectList)
		{
			if (@object.LevelObject.Type != LevelObjectType.NODRAW && bounds.Intersects(@object.Collider.bounds))
			{
				list.Add(@object);
			}
		}
		return list;
	}

	private void updateMultiSelectPositions()
	{
		m_multiSelectPosition.Clear();
		foreach (TouchDrawObject multiSelect in m_multiSelectList)
		{
			m_multiSelectPosition.Add(multiSelect.Position);
		}
	}
}
