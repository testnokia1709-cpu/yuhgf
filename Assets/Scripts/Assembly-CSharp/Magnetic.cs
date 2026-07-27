using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Magnetic : MonoBehaviour
{
	[HideInInspector]
	public Rigidbody2D Body;

	[HideInInspector]
	public Magnet Magnet;

	private void Awake()
	{
		GameObject gameObject = base.gameObject;
		while (gameObject.GetComponent<Rigidbody2D>() == null && !(gameObject.transform.parent == null))
		{
			gameObject = gameObject.transform.parent.gameObject;
		}
		Body = gameObject.GetComponent<Rigidbody2D>();
		if (Body == null)
		{
			base.enabled = false;
		}
		Magnet = base.gameObject.GetComponentInChildren<Magnet>();
	}
}
