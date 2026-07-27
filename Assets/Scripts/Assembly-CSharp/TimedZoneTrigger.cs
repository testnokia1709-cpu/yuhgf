using System.Collections.Generic;
using UnityEngine;

public class TimedZoneTrigger : MonoBehaviour
{
	public GameObject FilterObject;

	public LayerMask FilterLayer;

	public int GoalCount = 1;

	public bool GreaterThanOrEqual;

	public bool LessThanOrEqual;

	public bool EqualTo = true;

	public bool IgnoreKinematic = true;

	public List<TimedZoneTrigger> DependentTriggers;

	private bool m_isSolved;

	private bool m_countdownStarted;

	private int m_countInside;

	private bool m_filterObjectSet;

	private void Start()
	{
		m_isSolved = false;
		m_countdownStarted = false;
		m_countInside = 0;
		m_filterObjectSet = FilterObject != null;
	}

	public void SetFilterObject(GameObject obj)
	{
		FilterObject = obj;
		m_filterObjectSet = FilterObject != null;
	}

	public void SetGreaterThanOrEqual()
	{
		GreaterThanOrEqual = true;
		LessThanOrEqual = false;
		EqualTo = false;
	}

	public void SetEqualTo()
	{
		GreaterThanOrEqual = false;
		LessThanOrEqual = false;
		EqualTo = true;
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		Rigidbody2D component = col.gameObject.GetComponent<Rigidbody2D>();
		if (col.isTrigger || (IgnoreKinematic && (component == null || component.isKinematic)))
		{
			return;
		}
		LayerMask layerMask = 1 << col.gameObject.layer;
		if ((FilterLayer.value == 0 || (layerMask.value & FilterLayer.value) == FilterLayer.value) && (!m_filterObjectSet || !(col.gameObject != FilterObject)) && !TouchDrawPhysics.Instance.IsDrawShape(col.gameObject))
		{
			m_countInside++;
			if (GreaterThanOrEqual && m_countInside >= GoalCount)
			{
				m_isSolved = true;
			}
			else if (LessThanOrEqual && m_countInside <= GoalCount)
			{
				m_isSolved = true;
			}
			else if (EqualTo && m_countInside == GoalCount)
			{
				m_isSolved = true;
			}
			else
			{
				m_isSolved = false;
			}
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		Rigidbody2D component = col.gameObject.GetComponent<Rigidbody2D>();
		if (col.isTrigger || (IgnoreKinematic && (component == null || component.isKinematic)))
		{
			return;
		}
		LayerMask layerMask = 1 << col.gameObject.layer;
		if ((FilterLayer.value == 0 || (layerMask.value & FilterLayer.value) == FilterLayer.value) && (!m_filterObjectSet || !(col.gameObject != FilterObject)) && !TouchDrawPhysics.Instance.IsDrawShape(col.gameObject))
		{
			m_countInside--;
			if (GreaterThanOrEqual && m_countInside >= GoalCount)
			{
				m_isSolved = true;
			}
			else if (LessThanOrEqual && m_countInside <= GoalCount)
			{
				m_isSolved = true;
			}
			else if (EqualTo && m_countInside == GoalCount)
			{
				m_isSolved = true;
			}
			else
			{
				m_isSolved = false;
			}
		}
	}

	private void Update()
	{
		if (CountdownController.Instance == null)
		{
			return;
		}
		if (m_isSolved && GameStateManager.Instance != null && (GameStateManager.Instance.State == GameState.Playing || GameStateManager.Instance.State == GameState.WaitForPlayerStart) && haveDependentsSolved())
		{
			if (!CountdownController.Instance.IsEnabled)
			{
				m_countdownStarted = true;
				CountdownController.Instance.StartCountdown(delegate
				{
					GameStateManager.Instance.SetState(GameState.Solved);
				});
			}
		}
		else if (CountdownController.Instance.IsEnabled && m_countdownStarted && (!m_isSolved || !haveDependentsSolved()))
		{
			CountdownController.Instance.StopCountdown();
		}
	}

	public bool InTriggerCondition()
	{
		return m_isSolved;
	}

	private bool inInitialCondition()
	{
		return !m_isSolved;
	}

	private bool haveDependentsSolved()
	{
		if (DependentTriggers.Count == 0)
		{
			return true;
		}
		bool result = true;
		foreach (TimedZoneTrigger dependentTrigger in DependentTriggers)
		{
			if (!dependentTrigger.m_isSolved)
			{
				result = false;
			}
		}
		return result;
	}
}
