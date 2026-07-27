using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Magnet : MonoBehaviour
{
	public bool Up = true;

	public float Strength = 1f;

	private Rigidbody2D m_body;

	private List<Magnetic> m_magneticObjects = new List<Magnetic>();

	private Collider2D m_forceArea;

	private float m_strength;

	private void Awake()
	{
		GameObject gameObject = base.gameObject;
		while (gameObject.GetComponent<Rigidbody2D>() == null && !(gameObject.transform.parent == null))
		{
			gameObject = gameObject.transform.parent.gameObject;
		}
		m_body = gameObject.GetComponent<Rigidbody2D>();
		if (m_body == null)
		{
			base.enabled = false;
		}
		m_forceArea = base.gameObject.GetComponent<Collider2D>();
		m_forceArea.isTrigger = true;
		m_strength = Strength * m_body.mass;
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		foreach (Magnetic magneticObject in m_magneticObjects)
		{
			Vector2 vector = magneticObject.Body.gameObject.transform.position - m_body.gameObject.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude > 0.0001f)
			{
				Vector2 vector2 = m_strength * vector.normalized / sqrMagnitude;
				Vector2 vector3 = vector2 * magneticObject.Body.mass;
				float num = 1f;
				if (magneticObject.Magnet != null && magneticObject.Magnet.Up == Up)
				{
					num = 0f - num;
				}
				magneticObject.Body.AddForce(vector3 * (0f - num), ForceMode2D.Force);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.isTrigger)
		{
			Magnetic component = col.GetComponent<Magnetic>();
			if (component != null)
			{
				m_magneticObjects.Add(component);
			}
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if (!col.isTrigger)
		{
			Magnetic component = col.GetComponent<Magnetic>();
			if (component != null)
			{
				m_magneticObjects.Remove(component);
			}
		}
	}
}
