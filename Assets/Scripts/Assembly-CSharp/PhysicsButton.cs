using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
	public UnityEvent OnClick;

	public Color NormalColor = Color.white;

	public Color MouseOverColor = Color.white;

	public Color DisableColor = Color.white;

	private SpriteRenderer m_spriteRenderer;

	private Animator m_animator;

	private bool m_active = true;

	public bool Active
	{
		set
		{
			m_active = value;
			Color color = NormalColor;
			string trigger = "Normal";
			if (!value)
			{
				color = DisableColor;
				trigger = "Disabled";
			}
			if (m_spriteRenderer != null)
			{
				m_spriteRenderer.color = color;
			}
			if (m_animator != null)
			{
				m_animator.SetTrigger(trigger);
			}
		}
	}

	private void Start()
	{
		m_spriteRenderer = base.gameObject.GetComponent<SpriteRenderer>();
		m_animator = base.gameObject.GetComponent<Animator>();
	}

	public void MouseDown()
	{
		if (m_active)
		{
			if (m_spriteRenderer != null)
			{
				m_spriteRenderer.color = MouseOverColor;
			}
			if (m_animator != null)
			{
				m_animator.SetTrigger("Pressed");
			}
		}
	}

	public void MouseUp()
	{
		if (m_active)
		{
			if (m_spriteRenderer != null)
			{
				m_spriteRenderer.color = NormalColor;
			}
			if (m_animator != null)
			{
				m_animator.SetTrigger("Normal");
			}
		}
	}
}
