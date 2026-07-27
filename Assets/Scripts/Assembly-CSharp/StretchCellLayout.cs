using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class StretchCellLayout : MonoBehaviour
{
	public bool StretchVertical;

	public bool StretchHorizontal;

	public int Rows;

	public int Columns;

	public bool SquareAspect;

	private void Start()
	{
		determineLayout();
	}

	private void determineLayout()
	{
		GridLayoutGroup component = GetComponent<GridLayoutGroup>();
		RectTransform rectTransform = (RectTransform)base.gameObject.transform;
		float num = (rectTransform.rect.width - component.spacing.x * (float)Columns) / (float)Columns;
		float num2 = (rectTransform.rect.height - component.spacing.y * (float)Rows) / (float)Rows;
		float num3 = Mathf.Min(num, num2);
		if (StretchHorizontal && !StretchVertical)
		{
			component.cellSize = new Vector2(num, component.cellSize.y);
		}
		else if (StretchVertical && !StretchHorizontal)
		{
			component.cellSize = new Vector2(component.cellSize.x, num2);
		}
		else if (StretchVertical && StretchHorizontal)
		{
			component.cellSize = new Vector2(num, num2);
		}
		else if (SquareAspect)
		{
			component.cellSize = new Vector2(num3, num3);
		}
	}
}
