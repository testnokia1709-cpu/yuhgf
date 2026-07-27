using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	public StringId StringId;

	[HideInInspector]
	public Text Text;

	private void Awake()
	{
		Text = GetComponent<Text>();
	}
}
