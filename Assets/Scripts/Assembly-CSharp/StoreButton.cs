using UnityEngine;
using UnityEngine.UI;

public class StoreButton : MonoBehaviour
{
	public Button Button;

	public Image ImageSale;

	public void SetVisible(bool visible)
	{
		Button.gameObject.SetActive(visible);
	}

	public void SetSale(bool onSale)
	{
		ImageSale.gameObject.SetActive(onSale);
	}
}
