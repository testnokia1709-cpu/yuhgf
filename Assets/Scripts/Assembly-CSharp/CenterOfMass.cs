using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CenterOfMass : MonoBehaviour
{
	public Vector2 Offset;

	private void Start()
	{
		Rigidbody2D component = GetComponent<Rigidbody2D>();
		component.centerOfMass = Offset;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position + new Vector3(Offset.x, Offset.y), 0.5f);
	}
}
