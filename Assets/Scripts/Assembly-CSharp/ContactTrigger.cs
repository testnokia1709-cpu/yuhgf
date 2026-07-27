using System.Collections.Generic;
using UnityEngine;

public class ContactTrigger : MonoBehaviour
{
	public GameObject FilterObject;

	public LayerMask FilterLayer;

	public List<ContactTrigger> DependentTriggers;

	public bool SameObjectMustTouchDependentTriggers;

	public bool ShowColor = true;

	public bool ShowColorOnlyWithAllDependents;

	public int ContactCount = 1;

	[HideInInspector]
	public bool IsChild;

	[HideInInspector]
	public bool IsTriggered;

	private Renderer m_renderer;

	private bool m_filterObjectSet;

	private List<GameObject> m_contactObjects;

	private static int s_triggerCount;

	private int m_contactCount;

	private int m_flashColor;

	private void Awake()
	{
		m_renderer = base.gameObject.GetComponent<Renderer>();
		m_contactObjects = new List<GameObject>();
		m_filterObjectSet = FilterObject != null;
		s_triggerCount = 0;
	}

	private void Start()
	{
		if (DependentTriggers.Count == 0)
		{
			IsChild = true;
		}
	}

	private void Update()
	{
		if (m_flashColor > 0)
		{
			m_flashColor--;
			if (m_flashColor == 0 && m_renderer != null)
			{
				m_renderer.material.color = Color.white;
			}
		}
	}

	public void SetFilterObject(GameObject obj)
	{
		FilterObject = obj;
		m_filterObjectSet = true;
	}

	public void Reset()
	{
		base.enabled = false;
		IsTriggered = false;
		m_renderer.material.color = Color.white;
		m_contactCount = 0;
		m_contactObjects.Clear();
		DependentTriggers.Clear();
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (!base.enabled)
		{
			return;
		}
		Rigidbody2D component = col.gameObject.GetComponent<Rigidbody2D>();
		if (component == null)
		{
			Transform parent = col.gameObject.transform.parent;
			if (parent != null && parent.GetComponent<Rigidbody2D>() != null)
			{
				return;
			}
		}
		LayerMask layerMask = 1 << col.gameObject.layer;
		if ((FilterLayer.value == 0 || (layerMask.value & FilterLayer.value) == FilterLayer.value) && (!m_filterObjectSet || !(col.gameObject != FilterObject)))
		{
			contact(col.gameObject);
		}
	}

	private void contact(GameObject obj)
	{
		m_contactCount++;
		if (!m_contactObjects.Contains(obj))
		{
			m_contactObjects.Add(obj);
		}
		if (ShowColor && SameObjectMustTouchDependentTriggers && m_renderer != null)
		{
			int num = 0;
			foreach (ContactTrigger dependentTrigger in DependentTriggers)
			{
				if (dependentTrigger.m_contactObjects.Contains(obj))
				{
					num++;
				}
			}
			if (num >= s_triggerCount)
			{
				s_triggerCount = num;
				if (m_renderer != null)
				{
					m_renderer.material.color = Color.red;
				}
				foreach (ContactTrigger dependentTrigger2 in DependentTriggers)
				{
					if (dependentTrigger2.m_renderer != null)
					{
						if (dependentTrigger2.m_contactObjects.Contains(obj))
						{
							dependentTrigger2.m_renderer.material.color = Color.red;
						}
						else
						{
							dependentTrigger2.m_renderer.material.color = Color.white;
						}
					}
				}
			}
		}
		if (!IsTriggered && (!SameObjectMustTouchDependentTriggers || (SameObjectMustTouchDependentTriggers && sameObjectInDependentsContactList(obj))))
		{
			if (m_contactCount >= ContactCount)
			{
				IsTriggered = true;
			}
			else
			{
				m_flashColor = 10;
				if (m_renderer != null)
				{
					m_renderer.material.color = Color.red;
				}
			}
		}
		if (!IsTriggered || GameStateManager.Instance.State == GameState.Solved)
		{
			return;
		}
		if (ShowColor && !ShowColorOnlyWithAllDependents && !SameObjectMustTouchDependentTriggers && m_renderer != null)
		{
			m_renderer.material.color = Color.red;
		}
		if (!checkDependentsInTriggerCondition() && !SameObjectMustTouchDependentTriggers)
		{
			return;
		}
		if (ShowColor && (SameObjectMustTouchDependentTriggers || ShowColorOnlyWithAllDependents))
		{
			foreach (ContactTrigger dependentTrigger3 in DependentTriggers)
			{
				if (dependentTrigger3.m_renderer != null)
				{
					dependentTrigger3.m_renderer.material.color = Color.red;
				}
			}
			if (m_renderer != null)
			{
				m_renderer.material.color = Color.red;
			}
		}
		GameStateManager.Instance.SetState(GameState.Solved);
	}

	private bool checkDependentsInTriggerCondition()
	{
		if (DependentTriggers.Count == 0)
		{
			return true;
		}
		bool result = true;
		foreach (ContactTrigger dependentTrigger in DependentTriggers)
		{
			if (!dependentTrigger.IsTriggered)
			{
				result = false;
			}
		}
		return result;
	}

	private bool sameObjectInDependentsContactList(GameObject obj)
	{
		if (DependentTriggers.Count == 0)
		{
			return false;
		}
		bool result = true;
		foreach (ContactTrigger dependentTrigger in DependentTriggers)
		{
			if (!dependentTrigger.m_contactObjects.Contains(obj))
			{
				result = false;
			}
		}
		return result;
	}
}
