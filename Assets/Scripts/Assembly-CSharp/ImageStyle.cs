using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class ImageStyle : MonoBehaviour
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
			Image component = base.gameObject.GetComponent<Image>();
			component.color = Style.GetColor(StyleColor);
		}
	}
}
