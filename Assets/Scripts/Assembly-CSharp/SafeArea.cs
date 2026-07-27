using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
	public bool EnforceMaxAspectRatio;

	public float MaxAspectRatio = 0.5f;

	private void Start()
	{
		Rect safeArea = Screen.safeArea;
		RectTransform component = GetComponent<RectTransform>();
		float num = component.rect.width / component.rect.height;
		if (EnforceMaxAspectRatio)
		{
			float num2 = safeArea.width / safeArea.height;
			if (num2 < MaxAspectRatio)
			{
				safeArea.height = safeArea.width * (1f / MaxAspectRatio);
			}
		}
		if (component.rect.width > safeArea.width)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, safeArea.width);
		}
		if (component.rect.height > safeArea.height)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, safeArea.height);
		}
	}
}
