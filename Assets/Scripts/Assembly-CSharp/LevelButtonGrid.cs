using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class LevelButtonGrid : MonoBehaviour
{
	public GameObject TileTemplate;

	public int TileCount = 20;

	public int ColumnCount = 5;

	public List<GameObject> Tiles = new List<GameObject>();

	private GridLayoutGroup m_layout;

	private void Awake()
	{
		m_layout = base.gameObject.GetComponent<GridLayoutGroup>();
	}

	private void Start()
	{
		CreateTiles();
	}

	public void CreateTiles()
	{
		ClearTiles();
		RectTransform rectTransform = (RectTransform)base.gameObject.transform;
		int num = TileCount / ColumnCount;
		float a = (rectTransform.rect.width - m_layout.spacing.x * (float)ColumnCount) / (float)ColumnCount;
		float b = (rectTransform.rect.height - m_layout.spacing.y * (float)num) / (float)num;
		float num2 = Mathf.Min(a, b);
		m_layout.cellSize = new Vector2(num2, num2);
		for (int i = 0; i < TileCount; i++)
		{
			GameObject gameObject = Object.Instantiate(TileTemplate);
			gameObject.transform.SetParent(base.gameObject.transform);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			Tiles.Add(gameObject);
		}
	}

	public void ClearTiles()
	{
		foreach (GameObject tile in Tiles)
		{
			Object.DestroyObject(tile);
		}
		Tiles.Clear();
	}
}
