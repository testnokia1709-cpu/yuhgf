using UnityEngine;

public class BounceContact : MonoBehaviour
{
	private Animator m_animator;

	private void Start()
	{
		m_animator = base.gameObject.GetComponentInChildren<Animator>();
	}

	private void Update()
	{
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (m_animator != null)
		{
			m_animator.SetTrigger("Bounce");
		}
	}
}
