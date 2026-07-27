using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
	public static HintController Instance;

	public GameObject Hints;

	public GameObject Objects;

	public GameObject Shapes;

	public List<GameObject> Walls;

	private static Dictionary<string, bool> s_wasShown = new Dictionary<string, bool>();

	private static bool m_endHintPoint = false;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Hide();
		if (Objects == null)
		{
			Debug.LogError("Level Object Parent is null!");
		}
		if (Hints == null)
		{
			Debug.LogError("No hints defined!");
		}
	}

	public static bool wasShown()
	{
		return s_wasShown.ContainsKey(LevelManager.Instance.GetLevelKey());
	}

	public static void Show()
	{
		if (Instance == null)
		{
			return;
		}
		if (LevelManager.CommunityLevel || TouchDrawEditor.Instance != null)
		{
			List<Vector2> list = ((!LevelManager.CommunityLevel) ? TouchDrawEditor.Instance.Level.Hint : CommunityManager.CurrentLevel.Hint);
			if (list != null)
			{
				TouchDrawPhysics.Instance.StartDrawHintObject(Instance.Hints);
				Vector2 vector = Vector2.zero;
				bool flag = true;
				m_endHintPoint = false;
				foreach (Vector2 item in list)
				{
					if (!flag)
					{
						if (item == vector)
						{
							m_endHintPoint = true;
						}
						else if (m_endHintPoint)
						{
							vector = item;
							m_endHintPoint = false;
						}
					}
					TouchDrawPhysics.Instance.DrawHintObject(item, (!flag) ? vector : item);
					vector = item;
					flag = false;
				}
			}
		}
		string text = null;
		if (LevelManager.CommunityLevel)
		{
			text = CommunityManager.CurrentLevelId;
		}
		else if (TouchDrawEditor.Instance == null)
		{
			text = LevelManager.Instance.GetLevelKey();
		}
		if (text != null && !s_wasShown.ContainsKey(text))
		{
			s_wasShown.Add(text, true);
		}
		if (Instance.Objects != null)
		{
			SetVisibleRecursively(Instance.Objects, false);
		}
		if (Instance.Shapes != null)
		{
			SetVisibleRecursively(Instance.Shapes, false);
		}
		if (Instance.Walls != null)
		{
			foreach (GameObject wall in Instance.Walls)
			{
				SetVisibleRecursively(wall, false);
			}
		}
		if (Instance.Hints != null)
		{
			SetVisibleRecursively(Instance.Hints, true);
		}
	}

	public static void Hide()
	{
		if (Instance == null)
		{
			return;
		}
		if (LevelManager.CommunityLevel || TouchDrawEditor.Instance != null)
		{
			TouchDrawPhysics.Instance.ClearHint();
		}
		if (Instance.Objects != null)
		{
			SetVisibleRecursively(Instance.Objects, true);
		}
		if (Instance.Shapes != null)
		{
			SetVisibleRecursively(Instance.Shapes, true);
		}
		if (Instance.Walls != null)
		{
			foreach (GameObject wall in Instance.Walls)
			{
				SetVisibleRecursively(wall, true);
			}
		}
		if (Instance.Hints != null)
		{
			SetVisibleRecursively(Instance.Hints, false);
		}
	}

	private static void SetVisibleRecursively(GameObject obj, bool visible)
	{
		Renderer component = obj.GetComponent<Renderer>();
		if (component != null)
		{
			component.enabled = visible;
		}
		Image component2 = obj.GetComponent<Image>();
		if (component2 != null)
		{
			component2.enabled = visible;
		}
		foreach (Transform item in obj.transform)
		{
			SetVisibleRecursively(item.gameObject, visible);
		}
	}
}
