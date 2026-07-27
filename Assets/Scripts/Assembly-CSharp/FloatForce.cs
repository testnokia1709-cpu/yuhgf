using UnityEngine;

public class FloatForce : MonoBehaviour
{
	public float ForceMultiplier = 1.35f;

	private Rigidbody2D m_body;

	private float m_force;

	private static float s_gravity = -9.8f;

	private void Awake()
	{
		m_body = GetComponent<Rigidbody2D>();
		SetAntiGravForce(ForceMultiplier);
	}

	private void FixedUpdate()
	{
		if (m_body != null)
		{
			Vector2 force = new Vector2(0f, m_force * m_body.mass);
			m_body.AddForce(force);
		}
	}

	public void SetAntiGravForce(float force)
	{
		ForceMultiplier = force;
		m_force = (0f - s_gravity) * ForceMultiplier;
	}
}
