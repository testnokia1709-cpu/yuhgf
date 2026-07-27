using UnityEngine;

public class DestructionTrigger : MonoBehaviour
{
	public GameObject FilterObject;

	public LayerMask FilterLayer;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		GameObject gameObject = col.gameObject;
		Transform parent = gameObject.transform.parent;
		if (parent != null && parent.gameObject.GetComponent<Rigidbody2D>() != null)
		{
			gameObject = parent.gameObject;
		}
		if ((FilterLayer.value == 0 || gameObject.layer == FilterLayer.value) && (!(FilterObject != null) || !(gameObject != FilterObject)))
		{
			Object.Destroy(gameObject);
		}
	}
}
