using NBidi;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RTLText : MonoBehaviour
{
	private void Start()
	{
		Text component = base.gameObject.GetComponent<Text>();
		component.text = global::NBidi.NBidi.LogicalToVisual(component.text);
	}
}
