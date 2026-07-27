using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DetectGearJam : MonoBehaviour
{
	public List<DetectGearJam> DependentGears;

	private bool m_isJammed;

	private Rigidbody2D m_body;

	private static float s_tolerance = 0.05f;

	private void Start()
	{
		m_body = base.gameObject.GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		m_isJammed = Mathf.Abs(m_body.angularVelocity) < s_tolerance && GameStateManager.Instance.State == GameState.Playing;
		if (m_isJammed && checkDependentsAreJammed())
		{
			if (!CountdownController.Instance.IsEnabled)
			{
				CountdownController.Instance.StartCountdown(delegate
				{
					GameStateManager.Instance.SetState(GameState.Solved);
				});
			}
		}
		else if (CountdownController.Instance.IsEnabled)
		{
			CountdownController.Instance.StopCountdown();
		}
	}

	private bool checkDependentsAreJammed()
	{
		if (DependentGears.Count == 0)
		{
			return true;
		}
		bool result = true;
		foreach (DetectGearJam dependentGear in DependentGears)
		{
			if (dependentGear != null && !dependentGear.m_isJammed)
			{
				result = false;
			}
		}
		return result;
	}
}
