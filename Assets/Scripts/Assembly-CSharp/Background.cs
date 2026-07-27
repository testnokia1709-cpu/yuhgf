using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
	public void Start()
	{
		int index = 0;
		if (!LevelManager.CommunityLevel)
		{
			index = (LevelManager.Level - 1) / 20 % 3;
		}
		SpriteRenderer component = base.gameObject.GetComponent<SpriteRenderer>();
		if (UIManager.Instance != null)
		{
			component.sprite = UIManager.Instance.LevelTextures[index];
		}
	}
}
