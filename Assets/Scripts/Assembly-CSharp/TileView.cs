using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
	public GameObject TileTemplate;

	public GameObject BuyButton;

	public Button ButtonPreviousPage;

	public Button ButtonNextPage;

	public ImageBar PageBar;

	public int Padding;

	public bool ResizeContainerHeight = true;

	public bool FixedColumns;

	public int ColumnCount = 5;

	public bool FixedRows;

	public int RowCount = 3;

	public bool UsePages;

	public int ItemsPerPage = 15;

	public bool StretchTiles;

	public bool CenterHorizontally;

	public bool UsePageButtons = true;

	public Action<int, GameObject> TileCustomizer;

	public Action<int, GameObject> BuyButtonCustomizer;

	[HideInInspector]
	public bool IsScrolling;

	public Action OnScrollPage;

	public AnimationCurve ScrollCurve;

	public bool ScrollEnabled = true;

	private List<GameObject> m_tiles = new List<GameObject>();

	private List<GameObject> m_buyButtons = new List<GameObject>();

	private int m_pageIndex;

	private int m_pageCount;

	private Vector2 m_scrollStartPosition;

	private Vector2 m_scrollOffset;

	private float m_scrollDirection;

	private static float s_scrollDeadZone = (float)Screen.width * 0.03f;

	private static float s_scrollAutoZone = (float)Screen.width * 0.08f;

	private bool m_autoScrolling;

	private float m_autoScrollStart;

	private float m_autoScrollEnd;

	private float m_autoScrollStartTime;

	private bool m_validMouseDown;

	public int PageIndex
	{
		get
		{
			return m_pageIndex;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		PageBar.gameObject.SetActive(UsePages);
	}

	private void Update()
	{
		if (!ScrollEnabled || DialogManager.Instance.IsShown)
		{
			return;
		}
		bool flag = Input.GetMouseButtonDown(0);
		bool flag2 = Input.GetMouseButtonUp(0);
		bool flag3 = Input.GetMouseButton(0);
		Vector2 vector = Input.mousePosition;
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.touches[0];
			flag = touch.phase == TouchPhase.Began;
			flag2 = touch.phase == TouchPhase.Ended;
			flag3 = touch.phase == TouchPhase.Moved;
			vector = touch.position;
		}
		if (flag)
		{
			Vector3[] fourCornersArray = new Vector3[4];
			((RectTransform)base.gameObject.transform.parent).GetWorldCorners(fourCornersArray);
			Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			m_validMouseDown = IsMouseInsideRect(worldMousePosition, fourCornersArray);
			flag = m_validMouseDown;
		}
		if (!UsePages)
		{
			return;
		}
		if (flag && !m_autoScrolling)
		{
			m_scrollStartPosition = vector;
		}
		else if (flag2 && !m_autoScrolling)
		{
			m_autoScrollStart = (float)(-m_pageIndex * Screen.width) + m_scrollOffset.x;
			if (Mathf.Abs(m_scrollOffset.x) > s_scrollDeadZone)
			{
				m_autoScrolling = true;
				if (m_scrollDirection == 1f && m_scrollOffset.x > s_scrollAutoZone)
				{
					if (ShowPreviousPage() && OnScrollPage != null)
					{
						OnScrollPage();
					}
				}
				else if (m_scrollDirection == -1f && m_scrollOffset.x < 0f - s_scrollAutoZone && ShowNextPage() && OnScrollPage != null)
				{
					OnScrollPage();
				}
				m_autoScrollEnd = -m_pageIndex * Screen.width;
				m_autoScrollStartTime = Time.time;
			}
		}
		if (m_autoScrolling)
		{
			float num = (Time.time - m_autoScrollStartTime) * 2f;
			float x = Mathf.Lerp(m_autoScrollStart, m_autoScrollEnd, ScrollCurve.Evaluate(num));
			updatePagePosition(new Vector2(x, 0f));
			if (num >= 1f)
			{
				m_scrollDirection = 0f;
				m_scrollOffset.x = 0f;
				IsScrolling = false;
				m_autoScrolling = false;
			}
		}
		if (!flag3 || !m_validMouseDown || m_autoScrolling)
		{
			return;
		}
		m_scrollOffset = vector - m_scrollStartPosition;
		if (IsScrolling || Mathf.Abs(m_scrollOffset.x) > s_scrollDeadZone)
		{
			IsScrolling = true;
			m_scrollDirection = ((m_scrollOffset.x > 0f) ? 1 : (-1));
			if (((m_scrollDirection == 1f && m_pageIndex == 0) || (m_scrollDirection == -1f && m_pageIndex == m_pageCount - 1)) && Mathf.Abs(m_scrollOffset.x) > s_scrollAutoZone)
			{
				m_scrollOffset = new Vector2(s_scrollAutoZone * m_scrollDirection, 0f);
			}
			updatePagePosition(new Vector2(m_scrollOffset.x - (float)(m_pageIndex * Screen.width), 0f));
		}
	}

	private void updatePagePosition(Vector2 offset)
	{
		((RectTransform)base.gameObject.transform).anchoredPosition = offset;
	}

	public void SetFocusTile(int tileIndex)
	{
		if (UsePages)
		{
			m_pageIndex = (tileIndex - 1) / ItemsPerPage;
			updatePagePosition(new Vector2(-(m_pageIndex * Screen.width), 0f));
			updatePageButtons();
		}
	}

	public void CreateTiles(int tileCount)
	{
		m_pageCount = ((!UsePages) ? 1 : Mathf.CeilToInt((float)tileCount / (float)ItemsPerPage));
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		RectTransform component2 = TileTemplate.GetComponent<RectTransform>();
		int num = Mathf.FloorToInt(component.rect.width / (component2.rect.width + (float)(Padding * 2)));
		int num2 = Mathf.FloorToInt(component.rect.height / (component2.rect.height + (float)(Padding * 2)));
		if (FixedColumns)
		{
			num = ColumnCount;
		}
		int num3 = Mathf.CeilToInt((float)tileCount / (float)num);
		if (num2 != num3)
		{
			num2 = num3;
		}
		if (UsePages)
		{
			if (FixedRows)
			{
				num2 = RowCount;
			}
			else
			{
				int num4 = ItemsPerPage / ColumnCount;
				if (num2 > num4)
				{
					num2 = num4;
				}
			}
		}
		RectTransform component3 = base.gameObject.transform.parent.gameObject.GetComponent<RectTransform>();
		if (ResizeContainerHeight)
		{
			float num5 = (float)num3 * (component2.rect.height + (float)(Padding * 2)) + (float)(Padding * 2);
			if (num5 < component3.rect.height || (UsePages && num5 > component3.rect.height))
			{
				num5 = component3.rect.height;
			}
			component.sizeDelta = new Vector2(component.sizeDelta.x, num5);
		}
		Vector2 sizeDelta = new Vector2(component2.rect.width, component2.rect.height);
		if (StretchTiles)
		{
			float num6 = (float)(Padding * 2) + (float)num * (sizeDelta.x + (float)(Padding * 2));
			float num7 = (float)(Padding * 2) + (float)num2 * (sizeDelta.y + (float)(Padding * 2));
			float num8 = component.rect.width - num6;
			float num9 = component.rect.height - num7;
			float num10 = num8 / (float)num;
			float num11 = num9 / (float)num2;
			if (num10 <= num11)
			{
				float num12 = sizeDelta.x + num10;
				sizeDelta = new Vector2(num12, num12);
			}
			else if (num11 < num10)
			{
				float num13 = sizeDelta.y + num11;
				sizeDelta = new Vector2(num13, num13);
			}
		}
		float num14 = 0f;
		if (CenterHorizontally)
		{
			float num15 = (float)(Padding * 2) + (float)num * (sizeDelta.x + (float)(Padding * 2));
			float num16 = component.rect.width - num15;
			if (num16 > 0f)
			{
				num14 = num16 / 2f;
			}
		}
		float num17 = num14 + (float)Padding;
		float num18 = 0f;
		Debug.Log("Creating new tile objects...");
		for (int i = 0; i < tileCount; i++)
		{
			if (UsePages && i % ItemsPerPage == 0)
			{
				num18 = -(Padding * 2);
			}
			num17 += (float)Padding;
			GameObject gameObject = UnityEngine.Object.Instantiate(TileTemplate);
			gameObject.transform.SetParent(base.transform, false);
			m_tiles.Add(gameObject);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
			rectTransform.anchoredPosition = new Vector2(num17, num18);
			rectTransform.sizeDelta = sizeDelta;
			num17 += sizeDelta.x;
			num17 += (float)Padding;
			if ((i + 1) % num == 0)
			{
				num18 -= sizeDelta.y;
				num18 -= (float)(Padding * 2);
				num17 = num14 + (float)Padding;
				num17 += (float)(Screen.width * ((i + 1) / ItemsPerPage));
			}
		}
		Debug.Log("Customizing the tiles...");
		int num19 = 0;
		foreach (GameObject tile in m_tiles)
		{
			if (TileCustomizer != null)
			{
				TileCustomizer(num19 + 1, tile);
			}
			num19++;
		}
		m_buyButtons = new List<GameObject>();
		for (int j = 0; j < m_pageCount; j++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(BuyButton);
			gameObject2.transform.SetParent(base.transform, false);
			RectTransform rectTransform2 = (RectTransform)gameObject2.transform;
			rectTransform2.anchoredPosition = new Vector2(j * Screen.width, 0f);
			m_buyButtons.Add(gameObject2);
			if (BuyButtonCustomizer != null)
			{
				BuyButtonCustomizer(j, gameObject2);
			}
		}
		UnityEngine.Object.Destroy(BuyButton);
	}

	public void UpdateTiles()
	{
		if (TileCustomizer == null)
		{
			return;
		}
		int num = 0;
		foreach (GameObject tile in m_tiles)
		{
			if (num / ItemsPerPage == m_pageIndex)
			{
				TileCustomizer(num + 1, tile);
			}
			num++;
		}
	}

	public void UpdateBuyButtons()
	{
		if (BuyButtonCustomizer == null)
		{
			return;
		}
		int num = 0;
		foreach (GameObject buyButton in m_buyButtons)
		{
			BuyButtonCustomizer(num, buyButton);
			num++;
		}
	}

	public GameObject GetTile(int index)
	{
		return m_tiles[index];
	}

	public bool ShowPage(int index)
	{
		bool flag = false;
		if (index < m_pageCount)
		{
			float autoScrollStart = -m_pageIndex * Screen.width;
			m_pageIndex = index;
			flag = true;
			if (!m_autoScrolling)
			{
				m_autoScrolling = true;
				m_autoScrollStart = autoScrollStart;
				m_autoScrollEnd = -m_pageIndex * Screen.width;
				m_autoScrollStartTime = Time.time;
			}
		}
		if (flag)
		{
			updatePageButtons();
		}
		return flag;
	}

	public bool ShowNextPage()
	{
		bool flag = false;
		if (m_pageIndex < m_pageCount - 1)
		{
			float autoScrollStart = -m_pageIndex * Screen.width;
			m_pageIndex++;
			flag = true;
			if (!m_autoScrolling)
			{
				m_autoScrolling = true;
				m_autoScrollStart = autoScrollStart;
				m_autoScrollEnd = -m_pageIndex * Screen.width;
				m_autoScrollStartTime = Time.time;
			}
		}
		if (flag)
		{
			updatePageButtons();
		}
		return flag;
	}

	public bool ShowPreviousPage()
	{
		bool flag = false;
		if (m_pageIndex > 0)
		{
			float autoScrollStart = -m_pageIndex * Screen.width;
			m_pageIndex--;
			flag = true;
			if (!m_autoScrolling)
			{
				m_autoScrolling = true;
				m_autoScrollStart = autoScrollStart;
				m_autoScrollEnd = -m_pageIndex * Screen.width;
				m_autoScrollStartTime = Time.time;
			}
		}
		if (flag)
		{
			updatePageButtons();
		}
		return flag;
	}

	public void UpdateBuyButton(int index, bool enabled)
	{
		if (index < m_buyButtons.Count)
		{
			m_buyButtons[index].SetActive(enabled);
		}
	}

	private void updatePageButtons()
	{
		PageBar.SelectedMask = 1 << m_pageIndex;
		if (ButtonPreviousPage != null)
		{
			ButtonPreviousPage.gameObject.SetActive(UsePageButtons && m_pageIndex != 0);
		}
		if (ButtonNextPage != null)
		{
			ButtonNextPage.gameObject.SetActive(UsePageButtons && m_pageIndex < m_pageCount - 1);
		}
	}

	private bool IsMouseInsideRect(Vector3 worldMousePosition, Vector3[] fourCornersArray)
	{
		float width = fourCornersArray[3].x - fourCornersArray[1].x;
		float height = fourCornersArray[1].y - fourCornersArray[3].y;
		return new Rect(fourCornersArray[0].x, fourCornersArray[0].y, width, height).Contains(worldMousePosition);
	}
}
