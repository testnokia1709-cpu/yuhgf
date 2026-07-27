using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteStyle : MonoBehaviour
{
	public StyleColor StyleColor;

	public StyleColors Style;

	public void OnValidate()
	{
		if (Application.isEditor)
		{
			applyColor();
		}
	}

	public void OnStart()
	{
		applyColor();
	}

	private void applyColor()
	{
		if (Style != null)
		{
			SpriteRenderer component = base.gameObject.GetComponent<SpriteRenderer>();
			component.color = Style.GetColor(StyleColor);
		}
	}
}
