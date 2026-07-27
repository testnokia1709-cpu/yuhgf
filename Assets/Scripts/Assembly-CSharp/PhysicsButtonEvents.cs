using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsButtonEvents : MonoBehaviour
{
	private List<PhysicsButton> m_buttonsDown = new List<PhysicsButton>();

	private void Start()
	{
	}

	private void Update()
	{
		if (DialogManager.Instance.IsShown)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D[] array = Physics2D.OverlapCircleAll(clickPosition, 0.5f);
			if (array.Length > 0)
			{
				Collider2D collider2D = array.OrderBy((Collider2D h) => Vector3.Distance(h.transform.position, clickPosition)).Last();
				PhysicsButton component = collider2D.gameObject.GetComponent<PhysicsButton>();
				if (component != null)
				{
					m_buttonsDown.Add(component);
					component.MouseDown();
				}
			}
		}
		if (!Input.GetMouseButtonUp(0) || m_buttonsDown.Count <= 0)
		{
			return;
		}
		foreach (PhysicsButton item in m_buttonsDown)
		{
			item.MouseUp();
			if (item.OnClick != null)
			{
				item.OnClick.Invoke();
			}
		}
		m_buttonsDown.Clear();
	}
}
