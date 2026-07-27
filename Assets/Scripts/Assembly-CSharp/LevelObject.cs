using UnityEngine;

public class LevelObject : MonoBehaviour
{
	public LevelObjectType Type;

	public Vector3 Offset;

	public GameObject Outline;

	public Sprite Sprite;

	public void ShowOutline(bool enabled)
	{
		if (Outline != null)
		{
			Outline.SetActive(enabled);
		}
	}
}
