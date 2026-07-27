using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using LibTessDotNet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class TouchDrawPhysics : MonoBehaviour
{
	public static TouchDrawPhysics Instance;

	public float ShapeScale = 2.5f;

	public PhysicsMaterial2D ShapePhysicsMaterial;

	public Material ShapeMaterial;

	public float OutlineThickness = 0.1f;

	public bool HasOutline = true;

	public GameObject LevelParent;

	public GameObject ShapesParent;

	public GameObject ObjectsParent;

	public GameObject Ground;

	public GameObject Ceiling;

	public GameObject WallLeft;

	public GameObject WallRight;

	public bool TouchEnabled = true;

	public PhysicsMaterial2D NormalPhysicsMaterial;

	public PhysicsMaterial2D BouncyPhysicsMaterial;

	public PhysicsMaterial2D IcyPhysicsMaterial;

	public PhysicsMaterial2D RoughPhysicsMaterial;

	public PhysicsMaterial2D FloatingPhysicsMaterial;

	public PhysicsMaterial2D RedPhysicsMaterial;

	public Material NormalMaterial;

	public Material BouncyMaterial;

	public Material IcyMaterial;

	public Material RoughMaterial;

	public Material FloatingMaterial;

	public Material RedMaterial;

	public Material OutlineMaterial;

	public Action<Vector2, PointType> OnRecordPoint;

	public Action<float> OnDrawShape;

	public Func<bool> CanDraw;

	public int PointLimit;

	private List<Vector3> m_drawPoints;

	private List<GameObject> m_drawShape;

	private List<GameObject> m_shapeList;

	private GameObject m_hintRoot;

	private Vector3 m_lastDrawPoint;

	private Vector3 m_lastlastDrawPoint;

	private float m_pointRadius;

	private float m_edgeTolerance;

	private Vector2 m_prev2DPosition;

	private GameObject m_rootObject;

	private bool m_startDrawValid;

	private bool m_drawingCut;

	private bool m_badInput;

	private DeviceOrientation m_deviceOrientation;

	private CollisionDetectionMode2D m_collisionMode;

	private Dictionary<Rigidbody2D, float> m_angularVelocity = new Dictionary<Rigidbody2D, float>();

	private Dictionary<Rigidbody2D, Vector2> m_velocity = new Dictionary<Rigidbody2D, Vector2>();

	private static float s_timeScale = 2f;

	private static float s_minimumDistance = 0.75f;

	private static float s_sharpAngle = 50f;

	private static float s_subtleAngle = 20f;

	private static float s_snapPadding = 0.2f;

	private static float s_pointRadiusPadding = 0.025f;

	private static float s_collisionPadding = 0.05f;

	private static readonly string s_ignoreRaycast = "Ignore Raycast";

	private static readonly int s_clipperPrecision = 256;

	private static Vector2 s_clampBottomLeft;

	private static Vector2 s_clampTopRight;

	public bool IsPhysicsEnabled
	{
		get
		{
			return Time.timeScale != 0f;
		}
	}

	public int ShapeCount { get; private set; }

	public float PointRadius
	{
		get
		{
			return m_pointRadius;
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
		m_shapeList = new List<GameObject>();
		m_drawShape = new List<GameObject>();
		m_drawPoints = new List<Vector3>();
		m_pointRadius = ShapeScale / 2f + s_pointRadiusPadding;
		m_edgeTolerance = m_pointRadius * 3f;
		m_deviceOrientation = Input.deviceOrientation;
		SetDefaultBoundary();
		ResetCounts();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!TouchEnabled)
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
		Vector2 position = Camera.main.ScreenToWorldPoint(vector);
		bool flag4 = isValidPosition(ref position);
		if (CanDraw != null && !CanDraw())
		{
			return;
		}
		GameState gameState = GameState.Playing;
		if (GameStateManager.Instance != null)
		{
			gameState = GameStateManager.Instance.State;
		}
		if (gameState == GameState.WaitForPlayerStart && flag && flag4)
		{
			GameStateManager.Instance.SetState(GameState.Playing);
			gameState = GameStateManager.Instance.State;
		}
		if (gameState != GameState.Playing)
		{
			return;
		}
		if (hasDeviceOrientationChanged())
		{
			EndDrawObject(position, false);
			m_badInput = true;
		}
		else if (flag && flag4)
		{
			m_prev2DPosition = position;
			if (!testCollide2DPath(ref position, m_prev2DPosition) && canDraw(position) && isValidPosition(ref position))
			{
				StartDrawObject(position);
				m_startDrawValid = true;
			}
		}
		else if (flag2)
		{
			if (!m_drawingCut && flag4 && !testCollide2DPath(ref position, m_prev2DPosition) && canDraw(position) && isValidPosition(ref position))
			{
				DrawObject(position);
			}
			clampPosition(ref position);
			EndDrawObject(position, false);
			m_startDrawValid = false;
			m_drawingCut = false;
		}
		else
		{
			if (!flag3)
			{
				return;
			}
			if (!m_drawingCut && (flag4 || m_startDrawValid))
			{
				if (!m_startDrawValid)
				{
					m_prev2DPosition = position;
				}
				m_startDrawValid = true;
				checkForPath(position, m_prev2DPosition);
				if (!testCollide2DPath(ref position, m_prev2DPosition) && canDraw(position) && isValidPosition(ref position))
				{
					DrawObject(position);
				}
			}
			m_prev2DPosition = position;
		}
	}

	public void ResetCounts()
	{
		ShapeCount = 0;
	}

	public void SetDefaultBoundary()
	{
		s_clampBottomLeft = new Vector2(-37.8f, -35.1f);
		s_clampTopRight = new Vector2(37.8f, 29.7f);
	}

	public void SetBoundary(Vector2 bottomLeft, Vector2 topRight)
	{
		s_clampBottomLeft = bottomLeft;
		s_clampTopRight = topRight;
	}

	public void OnApplicationFocus(bool focusStatus)
	{
		if (TouchEnabled && m_drawPoints != null && m_drawPoints.Count > 0)
		{
			EndDrawObject(new Vector2(0f, 0f), false);
		}
	}

	public void OnApplicationPause(bool pauseStatus)
	{
		if (TouchEnabled && m_drawPoints != null && m_drawPoints.Count > 0)
		{
			EndDrawObject(new Vector2(0f, 0f), false);
		}
	}

	public void EnablePhysics(bool enable)
	{
		if (!enable)
		{
			cacheRigidbodyMotion();
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = s_timeScale;
			restoreRigidbodyMotion();
		}
	}

	public void SetShapeMaterial(LevelPenType penType)
	{
		Material shapeMaterial = Instance.NormalMaterial;
		PhysicsMaterial2D shapePhysicsMaterial = Instance.NormalPhysicsMaterial;
		switch (penType)
		{
		case LevelPenType.Bouncy:
			shapeMaterial = Instance.BouncyMaterial;
			shapePhysicsMaterial = Instance.BouncyPhysicsMaterial;
			break;
		case LevelPenType.Icy:
			shapeMaterial = Instance.IcyMaterial;
			shapePhysicsMaterial = Instance.IcyPhysicsMaterial;
			break;
		case LevelPenType.Rough:
			shapeMaterial = Instance.RoughMaterial;
			shapePhysicsMaterial = Instance.RoughPhysicsMaterial;
			break;
		case LevelPenType.Floaty:
			shapeMaterial = Instance.FloatingMaterial;
			shapePhysicsMaterial = Instance.FloatingPhysicsMaterial;
			break;
		case LevelPenType.Red:
			shapeMaterial = Instance.RedMaterial;
			shapePhysicsMaterial = Instance.RedPhysicsMaterial;
			break;
		}
		ShapeMaterial = shapeMaterial;
		ShapePhysicsMaterial = shapePhysicsMaterial;
	}

	public void SetCollisionMode(CollisionDetectionMode2D collisionMode)
	{
		m_collisionMode = collisionMode;
	}

	public static Color GetPenColor(LevelPenType penType)
	{
		Color result = Color.white;
		switch (penType)
		{
		case LevelPenType.Bouncy:
			result = Color.magenta;
			break;
		case LevelPenType.Icy:
			result = Color.cyan;
			break;
		case LevelPenType.Rough:
			result = Color.green;
			break;
		case LevelPenType.Floaty:
			result = Color.yellow;
			break;
		case LevelPenType.Red:
			result = Color.red;
			break;
		}
		return result;
	}

	private void checkForPath(Vector2 position, Vector2 prevPosition)
	{
		if (Input.GetMouseButton(0))
		{
			Vector2 direction = position - prevPosition;
			float distance = Vector2.Distance(prevPosition, position);
			RaycastHit2D[] array = Physics2D.CircleCastAll(prevPosition, m_pointRadius, direction, distance);
			RaycastHit2D[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D = array2[i];
				pathAroundShape(raycastHit2D.collider.gameObject, raycastHit2D.point, position);
			}
		}
	}

	private void pathAroundShape(GameObject obj, Vector2 hitPoint, Vector2 position)
	{
		PolygonCollider2D component = obj.GetComponent<PolygonCollider2D>();
		if (component != null)
		{
			int pathCount = component.pathCount;
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < component.pathCount; i++)
			{
				list.AddRange(component.GetPath(i));
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = component.transform.position + component.transform.rotation * new Vector3(list[j].x * component.transform.lossyScale.x, list[j].y * component.transform.lossyScale.y, 0f);
			}
			createPath(list, hitPoint, position);
		}
	}

	private void createPath(List<Vector2> points, Vector2 hitPoint, Vector2 destPoint)
	{
		int num = 0;
		int num2 = 0;
		float num3 = 9999f;
		for (int i = 0; i < points.Count; i++)
		{
			float num4 = Vector2.Distance(hitPoint, points[i]);
			if (num4 < num3)
			{
				num = i;
				num3 = num4;
			}
		}
		Vector2 vector = hitPoint;
		num3 = 9999f;
		for (int j = 0; j < points.Count; j++)
		{
			float num5 = Vector2.Distance(destPoint, points[j]);
			if (num5 < num3)
			{
				vector = points[j];
				num2 = j;
				num3 = num5;
			}
		}
		Collider2D collider2D = Physics2D.OverlapPoint(destPoint);
		if (collider2D != null || (destPoint - vector).magnitude > m_pointRadius)
		{
			return;
		}
		int num6 = 0;
		int num7 = 0;
		if (num == num2)
		{
			return;
		}
		if (num2 > num)
		{
			num6 = num2 - num;
			num7 = points.Count - num2 + num;
		}
		else
		{
			num7 = num - num2;
			num6 = points.Count - num + num2;
		}
		int num8 = Mathf.Min(num6, num7);
		int num9 = 0;
		num9 = ((num6 < num7) ? 1 : (-1));
		List<Vector2> list = new List<Vector2>();
		Vector2 vector2 = default(Vector2);
		if (num9 != 0)
		{
			int num10 = 0;
			int num11 = 25;
			if (num8 < num11)
			{
				int num12 = num;
				while (num12 != num2 && num10++ < num11)
				{
					list.Add(points[num12]);
					vector2.x += points[num12].x;
					vector2.y += points[num12].y;
					num12 += num9;
					if (num9 > 0 && num12 > points.Count - 1)
					{
						num12 = 0;
					}
					else if (num9 < 0 && num12 < 0)
					{
						num12 = points.Count - 1;
					}
				}
			}
		}
		vector2.x /= list.Count;
		vector2.y /= list.Count;
		Vector2 prev2DPosition = m_prev2DPosition;
		if (list.Count > 0)
		{
			for (int k = 0; k < list.Count; k++)
			{
				Vector2 vector3 = vector2 + (list[k] - vector2) * 1.1f;
				RaycastHit2D raycastHit2D = Physics2D.Raycast(vector3, vector2 - vector3);
				Vector2 vector4 = list[k];
				list[k] += raycastHit2D.normal * (m_pointRadius + s_snapPadding);
			}
			prev2DPosition = m_prev2DPosition;
			for (int l = 0; l < list.Count; l++)
			{
				Vector2 position = list[l];
				drawPathPoint(ref position, prev2DPosition);
				prev2DPosition = position;
				list[l] = position;
			}
			m_prev2DPosition = prev2DPosition;
			clampPosition(ref m_prev2DPosition);
		}
	}

	private void drawPathPoint(ref Vector2 position, Vector2 prevPosition)
	{
		if (!testCollide2DPath(ref position, prevPosition) && canDraw(position))
		{
			DrawObject(position);
		}
	}

	private void debugDrawSquare(Vector3 position, float halfSize, Color color, float duration = 0f)
	{
		Debug.DrawLine(position + new Vector3(0f - halfSize, halfSize), position + new Vector3(halfSize, halfSize), color, duration);
		Debug.DrawLine(position + new Vector3(0f - halfSize, halfSize), position + new Vector3(0f - halfSize, 0f - halfSize), color, duration);
		Debug.DrawLine(position + new Vector3(halfSize, halfSize), position + new Vector3(halfSize, 0f - halfSize), color, duration);
		Debug.DrawLine(position + new Vector3(0f - halfSize, 0f - halfSize), position + new Vector3(halfSize, 0f - halfSize), color, duration);
	}

	private void debugDrawCross(Vector3 position, float halfSize, Color color, float duration = 0f)
	{
		Debug.DrawLine(position - new Vector3(halfSize, 0f), position + new Vector3(halfSize, 0f), color, duration);
		Debug.DrawLine(position - new Vector3(0f, halfSize), position + new Vector3(0f, halfSize), color, duration);
	}

	private bool canDraw(Vector2 position)
	{
		Collider2D[] array = Physics2D.OverlapCircleAll(position, m_pointRadius);
		Collider2D[] array2 = array;
		foreach (Collider2D collider2D in array2)
		{
			if ((bool)collider2D.gameObject.GetComponent<NoDrawTrigger>())
			{
				return false;
			}
		}
		return true;
	}

	private bool testCollide2DPath(ref Vector2 position, Vector2 prevPosition)
	{
		Vector2 vector = position - prevPosition;
		float distance = Vector2.Distance(prevPosition, position);
		bool flag = Physics2D.OverlapPoint(prevPosition) != null;
		bool flag2 = Physics2D.OverlapPoint(position) != null;
		if (flag2 && flag)
		{
			position = prevPosition;
			return true;
		}
		RaycastHit2D raycastHit2D = ((!flag || flag2) ? Physics2D.CircleCast(prevPosition, m_pointRadius, vector, distance) : Physics2D.CircleCast(position, m_pointRadius, -vector, distance));
		if ((bool)raycastHit2D)
		{
			Vector2 vector2 = raycastHit2D.point + raycastHit2D.normal * (m_pointRadius + s_snapPadding);
			vector = vector2 - prevPosition;
			distance = Vector2.Distance(prevPosition, vector2);
			raycastHit2D = Physics2D.CircleCast(prevPosition, m_pointRadius, vector, distance);
			position = vector2;
		}
		clampPosition(ref position);
		return raycastHit2D;
	}

	private bool isValidPosition(ref Vector2 position)
	{
		float num = Mathf.Clamp(position.x, s_clampBottomLeft.x, s_clampTopRight.x);
		float num2 = Mathf.Clamp(position.y, s_clampBottomLeft.y, s_clampTopRight.y);
		if (Math.Abs(position.x - num) > m_edgeTolerance || Math.Abs(position.y - num2) > m_edgeTolerance)
		{
			return false;
		}
		position = new Vector2(num, num2);
		return true;
	}

	private bool clampPosition(ref Vector2 position)
	{
		bool result = false;
		float num = Mathf.Clamp(position.x, s_clampBottomLeft.x, s_clampTopRight.x);
		float num2 = Mathf.Clamp(position.y, s_clampBottomLeft.y, s_clampTopRight.y);
		if (Math.Abs(position.x - num) > m_pointRadius || Math.Abs(position.y - num2) > m_pointRadius)
		{
			result = true;
		}
		position.x = num;
		position.y = num2;
		return result;
	}

	public void StartDrawObject(Vector2 position)
	{
		if (m_drawPoints.Count > 0)
		{
			EndDrawObject(position, false);
		}
		else if (PointLimit != 0 && GetPointCount() >= PointLimit)
		{
			return;
		}
		Vector3 vector = new Vector3(position.x, position.y, ShapesParent.transform.position.z);
		m_drawPoints.Clear();
		m_drawShape.Clear();
		m_rootObject = new GameObject();
		m_rootObject.transform.parent = ShapesParent.transform;
		m_rootObject.transform.position = vector;
		m_drawPoints.Add(vector);
		m_lastDrawPoint = vector;
		m_lastlastDrawPoint = m_lastDrawPoint;
		GameObject item = addShapeSegmentConnector(m_rootObject, vector);
		m_drawShape.Add(item);
		ShapeCount++;
		if (OnRecordPoint != null)
		{
			OnRecordPoint(new Vector2(position.x, position.y), PointType.Begin);
		}
	}

	public void StartDrawHintObject(GameObject hintsParent)
	{
		m_rootObject = new GameObject();
		m_rootObject.transform.parent = hintsParent.transform;
		m_rootObject.transform.position = Vector3.zero;
		m_hintRoot = m_rootObject;
	}

	public void DrawObject(Vector2 position)
	{
		if (m_drawPoints.Count == 0)
		{
			StartDrawObject(position);
			return;
		}
		if (PointLimit != 0 && GetPointCount() >= PointLimit)
		{
			EndDrawObject(position, false);
			return;
		}
		Vector3 vector = new Vector3(position.x, position.y, ShapesParent.transform.position.z);
		float num = Vector3.Distance(vector, m_lastDrawPoint);
		if (!(num > s_minimumDistance))
		{
			return;
		}
		m_drawPoints.Add(vector);
		GameObject gameObject = addShapeSegment(m_rootObject, vector, m_lastDrawPoint);
		GameObject gameObject2 = addShapeSegmentConnector(m_rootObject, vector);
		if (m_drawShape.Count > 1)
		{
			float angleOfLineBetweenTwoPoints = GetAngleOfLineBetweenTwoPoints(vector, m_lastDrawPoint);
			float angleOfLineBetweenTwoPoints2 = GetAngleOfLineBetweenTwoPoints(m_lastDrawPoint, m_lastlastDrawPoint);
			float num2 = Mathf.Abs(Mathf.DeltaAngle(angleOfLineBetweenTwoPoints, angleOfLineBetweenTwoPoints2));
			GameObject gameObject3 = m_drawShape.Last();
			if (num2 < s_subtleAngle)
			{
				m_drawShape.Remove(gameObject3);
				UnityEngine.Object.Destroy(gameObject3);
			}
		}
		m_lastlastDrawPoint = m_lastDrawPoint;
		m_lastDrawPoint = vector;
		gameObject.layer = LayerMask.NameToLayer(s_ignoreRaycast);
		gameObject2.layer = LayerMask.NameToLayer(s_ignoreRaycast);
		m_drawShape.Add(gameObject);
		m_drawShape.Add(gameObject2);
		if (OnRecordPoint != null)
		{
			OnRecordPoint(position, PointType.Middle);
		}
	}

	public void DrawHintObject(Vector2 position, Vector2 prevPosition)
	{
		Vector3 point = new Vector3(position.x, position.y, ShapesParent.transform.position.z);
		Vector3 previousPoint = new Vector3(prevPosition.x, prevPosition.y, ShapesParent.transform.position.z);
		GameObject gameObject = addShapeSegment(m_rootObject, point, previousPoint, true);
		GameObject gameObject2 = addShapeSegmentConnector(m_rootObject, point, true);
		gameObject.layer = LayerMask.NameToLayer(s_ignoreRaycast);
		gameObject2.layer = LayerMask.NameToLayer(s_ignoreRaycast);
	}

	public bool EndDrawObject(Vector2 position, bool addLastPoint = true)
	{
		bool result = false;
		if (m_drawPoints.Count > 0)
		{
			Vector3 vector = new Vector3(position.x, position.y, ShapesParent.transform.position.z);
			if (addLastPoint && Vector3.Distance(vector, m_lastDrawPoint) > s_minimumDistance)
			{
				m_drawPoints.Add(vector);
			}
			addShape(m_drawPoints, m_drawShape, m_rootObject);
			result = true;
			if (ShapeMaterial == FloatingMaterial)
			{
				m_rootObject.AddComponent<FloatForce>();
			}
			else if (ShapeMaterial == RedMaterial)
			{
				FloatForce floatForce = m_rootObject.AddComponent<FloatForce>();
				floatForce.SetAntiGravForce(1f);
			}
			m_drawShape.Clear();
			m_drawPoints.Clear();
			if (OnRecordPoint != null)
			{
				OnRecordPoint(position, PointType.End);
			}
		}
		return result;
	}

	public void ClearHint()
	{
		UnityEngine.Object.Destroy(m_hintRoot);
	}

	public int GetPointCount()
	{
		int num = 0;
		foreach (GameObject shape in m_shapeList)
		{
			num += shape.transform.childCount;
		}
		return num + m_drawShape.Count;
	}

	public bool IsDrawShape(GameObject obj)
	{
		return m_drawShape.Contains(obj);
	}

	private void addShape(List<Vector3> drawPoints, List<GameObject> shapeObjects, GameObject rootObject)
	{
		List<List<IntPoint>> solution = createPolygonWithClipper(drawPoints, rootObject);
		if (drawPoints.Count == 1)
		{
			CircleCollider2D circleCollider2D = rootObject.AddComponent<CircleCollider2D>();
			circleCollider2D.sharedMaterial = ShapePhysicsMaterial;
			circleCollider2D.radius = m_pointRadius;
		}
		else
		{
			createCollisionWithClipper(solution, rootObject);
		}
		foreach (GameObject shapeObject in shapeObjects)
		{
			UnityEngine.Object.Destroy(shapeObject.GetComponent<Collider2D>());
		}
		Rigidbody2D rigidbody2D = rootObject.AddComponent<Rigidbody2D>();
		rigidbody2D.collisionDetectionMode = m_collisionMode;
		float num = (float)drawPoints.Count / 2f * (ShapeScale / 2.5f);
		rigidbody2D.mass = Mathf.Max(num, 1f);
		m_shapeList.Add(rootObject);
		if (OnDrawShape != null)
		{
			OnDrawShape(num);
		}
		DataStore.Instance.ShapeCount++;
	}

	private void createCollisionWithClipper(List<List<IntPoint>> solution, GameObject rootObject)
	{
		PolygonCollider2D polygonCollider2D = rootObject.AddComponent<PolygonCollider2D>();
		polygonCollider2D.sharedMaterial = ShapePhysicsMaterial;
		polygonCollider2D.pathCount = solution.Count;
		int num = 0;
		foreach (List<IntPoint> item in solution)
		{
			List<Vector2> list = new List<Vector2>();
			foreach (IntPoint item2 in item)
			{
				list.Add(new Vector2((float)item2.X / (float)s_clipperPrecision, (float)item2.Y / (float)s_clipperPrecision));
			}
			Vector2[] points = list.ToArray();
			polygonCollider2D.SetPath(num++, points);
		}
	}

	private GameObject createCircleMesh(float size, Material mat)
	{
		float num = 0f;
		float num2 = 25f;
		int num3 = (int)(360f / num2);
		Vector2 v = new Vector2(0f, size);
		Vector3[] array = new Vector3[num3 + 1];
		for (int i = 0; i <= num3; i++)
		{
			Vector2 vector = v.Rotate(0f - num);
			array[i] = new Vector3(0f - vector.x, 0f - vector.y, 0f);
			num += num2;
		}
		GameObject gameObject = new GameObject();
		createMesh(gameObject, mat, array);
		return gameObject;
	}

	private GameObject createLineMesh(float width, float height, Material mat)
	{
		float num = height / 2f;
		float num2 = width / 2f;
		Vector3[] vertexList = new Vector3[4]
		{
			new Vector3(0f - num2, num),
			new Vector3(num2, num),
			new Vector3(num2, 0f - num),
			new Vector3(0f - num2, 0f - num)
		};
		GameObject gameObject = new GameObject();
		createMesh(gameObject, mat, vertexList);
		return gameObject;
	}

	private void createMesh(GameObject obj, Material mat, Vector3[] vertexList)
	{
		Tess tess = new Tess();
		ContourVertex[] array = new ContourVertex[vertexList.Length];
		for (int i = 0; i < vertexList.Length; i++)
		{
			array[i].Position = new Vec3
			{
				X = vertexList[i].x,
				Y = vertexList[i].y,
				Z = 0f
			};
		}
		tess.AddContour(array);
		tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
		Vector3[] array2 = new Vector3[tess.VertexCount];
		for (int j = 0; j < tess.VertexCount; j++)
		{
			array2[j] = new Vector3(tess.Vertices[j].Position.X, tess.Vertices[j].Position.Y, tess.Vertices[j].Position.Z);
		}
		int[] elements = tess.Elements;
		Mesh mesh = createMesh(array2, elements);
		addMeshRenderer(obj, mesh, mat);
	}

	private void createMesh(GameObject obj, Material mat, List<List<IntPoint>> solution)
	{
		Tess tess = new Tess();
		foreach (List<IntPoint> item in solution)
		{
			ContourVertex[] array = new ContourVertex[item.Count];
			for (int i = 0; i < item.Count; i++)
			{
				int index = item.Count - 1 - i;
				array[i].Position = new Vec3
				{
					X = (float)item[index].X / (float)s_clipperPrecision,
					Y = (float)item[index].Y / (float)s_clipperPrecision,
					Z = 0f
				};
			}
			tess.AddContour(array);
		}
		tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
		Vector3[] array2 = new Vector3[tess.VertexCount];
		for (int j = 0; j < tess.VertexCount; j++)
		{
			array2[j] = new Vector3(tess.Vertices[j].Position.X, tess.Vertices[j].Position.Y, tess.Vertices[j].Position.Z);
		}
		int[] elements = tess.Elements;
		Mesh mesh = createMesh(array2, elements);
		addMeshRenderer(obj, mesh, mat);
	}

	private Mesh createMesh(Vector3[] vertices, int[] indices)
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}

	private void addMeshRenderer(GameObject obj, Mesh mesh, Material mat)
	{
		MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		meshRenderer.material = mat;
		MeshFilter meshFilter = obj.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshFilter.mesh = mesh;
	}

	private List<List<IntPoint>> createPolygonWithClipper(List<Vector3> linePoints, GameObject rootObject)
	{
		List<List<IntPoint>> list = new List<List<IntPoint>>();
		List<IntPoint> list2 = null;
		Vector3 zero = Vector3.zero;
		float num = 0f;
		for (int i = 0; i < linePoints.Count; i++)
		{
			float num2 = num;
			num = ((i + 1 >= linePoints.Count) ? 0f : GetAngleOfLineBetweenTwoPoints(linePoints[i], linePoints[i + 1]));
			if (i == 0)
			{
				num2 = num;
				zero = linePoints[i];
			}
			else
			{
				zero = linePoints[i - 1];
			}
			bool flag = Mathf.Abs(Mathf.DeltaAngle(num, num2)) > s_sharpAngle;
			if (i + 1 < linePoints.Count)
			{
				list2 = getClipperPolygonFromPoints(linePoints[i] - rootObject.transform.position, linePoints[i + 1] - rootObject.transform.position, num, num2, flag);
				list.Add(list2);
			}
			if (i == 0 || i + 1 == linePoints.Count)
			{
				list.Add(getClipperCircleFromPoint(linePoints[i] - rootObject.transform.position));
			}
			else if (flag)
			{
				list.Add(getClipperCircleFromPoint(linePoints[i] - rootObject.transform.position));
			}
		}
		List<List<IntPoint>> list3 = new List<List<IntPoint>>();
		Clipper clipper = new Clipper();
		clipper.AddPaths(list, PolyType.ptSubject, true);
		clipper.Execute(ClipType.ctUnion, list3, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
		return list3;
	}

	private List<IntPoint> getClipperPolygonFromPoints(Vector2 startPoint, Vector2 endPoint, float angle, float prevAngle, bool sharpAngle)
	{
		List<IntPoint> list = new List<IntPoint>();
		Vector2 v = new Vector2(0f, 0f - m_pointRadius - s_collisionPadding);
		Vector2 vector = v.Rotate(angle);
		Vector2 vector2 = v.Rotate(prevAngle);
		list.Add(new IntPoint((startPoint.x - vector2.x) * (float)s_clipperPrecision, (startPoint.y - vector2.y) * (float)s_clipperPrecision));
		if (sharpAngle && angle < prevAngle)
		{
			Vector2 vector3 = v.Rotate((angle + prevAngle) / 2f);
			list.Add(new IntPoint((startPoint.x - vector3.x) * (float)s_clipperPrecision, (startPoint.y - vector3.y) * (float)s_clipperPrecision));
		}
		list.Add(new IntPoint((endPoint.x - vector.x) * (float)s_clipperPrecision, (endPoint.y - vector.y) * (float)s_clipperPrecision));
		list.Add(new IntPoint((endPoint.x + vector.x) * (float)s_clipperPrecision, (endPoint.y + vector.y) * (float)s_clipperPrecision));
		if (sharpAngle && angle > prevAngle)
		{
			Vector2 vector4 = v.Rotate((angle + prevAngle) / 2f);
			list.Add(new IntPoint((startPoint.x + vector4.x) * (float)s_clipperPrecision, (startPoint.y + vector4.y) * (float)s_clipperPrecision));
		}
		list.Add(new IntPoint((startPoint.x + vector2.x) * (float)s_clipperPrecision, (startPoint.y + vector2.y) * (float)s_clipperPrecision));
		return list;
	}

	private List<IntPoint> getClipperCircleFromPoint(Vector2 point)
	{
		float num = 0f;
		float num2 = 25f;
		int num3 = (int)(360f / num2);
		List<IntPoint> list = new List<IntPoint>(num3);
		Vector2 v = new Vector2(0f, 0f - m_pointRadius - s_collisionPadding);
		for (int i = 0; i <= num3; i++)
		{
			Vector2 vector = v.Rotate(0f - num);
			list.Add(new IntPoint((point.x - vector.x) * (float)s_clipperPrecision, (point.y - vector.y) * (float)s_clipperPrecision));
			num += num2;
		}
		return list;
	}

	private GameObject addShapeSegment(GameObject parent, Vector3 point, Vector3 previousPoint, bool removeCollision = false)
	{
		Vector3 position = Vector3.Lerp(previousPoint, point, 0.5f);
		GameObject gameObject = createLineMesh(Vector3.Distance(previousPoint, point) + 0.5f, m_pointRadius * 2f, ShapeMaterial);
		float angleOfLineBetweenTwoPoints = GetAngleOfLineBetweenTwoPoints(previousPoint, point);
		gameObject.transform.Rotate(0f, 0f, angleOfLineBetweenTwoPoints);
		gameObject.GetComponent<Renderer>().sortingOrder = 0;
		gameObject.layer = LayerMask.NameToLayer(s_ignoreRaycast);
		gameObject.transform.parent = parent.transform;
		gameObject.transform.position = position;
		if (!removeCollision)
		{
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.sharedMaterial = ShapePhysicsMaterial;
		}
		if (HasOutline)
		{
			GameObject gameObject2 = createLineMesh(Vector3.Distance(previousPoint, point) + 0.5f, m_pointRadius * 2f + OutlineThickness, OutlineMaterial);
			gameObject.GetComponent<Renderer>().sortingOrder = -1;
			gameObject2.transform.Rotate(0f, 0f, angleOfLineBetweenTwoPoints);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0.5f);
		}
		return gameObject;
	}

	private GameObject addShapeSegmentConnector(GameObject parent, Vector3 point, bool removeCollision = false)
	{
		GameObject gameObject = createCircleMesh(m_pointRadius, ShapeMaterial);
		gameObject.GetComponent<Renderer>().sortingOrder = 0;
		gameObject.layer = LayerMask.NameToLayer(s_ignoreRaycast);
		gameObject.transform.parent = parent.transform;
		gameObject.transform.position = point;
		if (!removeCollision)
		{
			CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
			circleCollider2D.sharedMaterial = ShapePhysicsMaterial;
			circleCollider2D.radius = m_pointRadius;
		}
		if (HasOutline)
		{
			GameObject gameObject2 = createCircleMesh(m_pointRadius + OutlineThickness / 2f, OutlineMaterial);
			gameObject.GetComponent<Renderer>().sortingOrder = -1;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0.5f);
		}
		return gameObject;
	}

	private bool hasDeviceOrientationChanged()
	{
		bool result = false;
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
		{
			if ((m_deviceOrientation == DeviceOrientation.LandscapeLeft && Input.deviceOrientation == DeviceOrientation.LandscapeRight) || (m_deviceOrientation == DeviceOrientation.LandscapeRight && Input.deviceOrientation == DeviceOrientation.LandscapeLeft))
			{
				result = true;
			}
			m_deviceOrientation = Input.deviceOrientation;
		}
		return result;
	}

	public static float GetAngleOfLineBetweenTwoPoints(Vector2 p1, Vector2 p2)
	{
		float x = p2.x - p1.x;
		float y = p2.y - p1.y;
		return Mathf.Atan2(y, x) * (180f / (float)Math.PI);
	}

	public static float GetAngleOfLineBetweenTwoPointsRad(Vector2 p1, Vector2 p2)
	{
		float x = p2.x - p1.x;
		float y = p2.y - p1.y;
		return Mathf.Atan2(y, x);
	}

	public void ClearShapes()
	{
		foreach (GameObject shape in m_shapeList)
		{
			UnityEngine.Object.Destroy(shape);
		}
		m_shapeList.Clear();
		UnityEngine.Object.Destroy(m_rootObject);
		foreach (GameObject item in m_drawShape)
		{
			UnityEngine.Object.Destroy(item);
		}
		m_drawShape.Clear();
		m_drawPoints.Clear();
	}

	public int GetAmountDrawingShapes()
	{
		return m_shapeList.Count;
	}

	public float GetDrawingShapesMass()
	{
		float num = 0f;
		foreach (GameObject shape in m_shapeList)
		{
			if (shape != null)
			{
				Rigidbody2D component = shape.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					num += component.mass;
				}
			}
		}
		return num;
	}

	private bool IsPointerOverUIObject()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		return list.Count > 0;
	}

	private void cacheRigidbodyMotion()
	{
		m_angularVelocity.Clear();
		m_velocity.Clear();
		Rigidbody2D[] array = UnityEngine.Object.FindObjectsOfType<Rigidbody2D>();
		Rigidbody2D[] array2 = array;
		foreach (Rigidbody2D rigidbody2D in array2)
		{
			m_angularVelocity.Add(rigidbody2D, rigidbody2D.angularVelocity);
			m_velocity.Add(rigidbody2D, rigidbody2D.velocity);
		}
	}

	private void restoreRigidbodyMotion()
	{
		foreach (KeyValuePair<Rigidbody2D, float> item in m_angularVelocity)
		{
			if (item.Key != null)
			{
				item.Key.angularVelocity = item.Value;
			}
		}
		foreach (KeyValuePair<Rigidbody2D, Vector2> item2 in m_velocity)
		{
			if (item2.Key != null)
			{
				item2.Key.velocity = item2.Value;
			}
		}
	}
}
