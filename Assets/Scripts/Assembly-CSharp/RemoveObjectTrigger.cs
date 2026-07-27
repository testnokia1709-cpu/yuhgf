using System;
using System.Collections.Generic;
using UnityEngine;

public class RemoveObjectTrigger : MonoBehaviour
{
	public float DelayTime;

	public float DelayMax;

	public float TimerHUDScale = 1f;

	public GameObject TimerHUDPrefab;

	private ObjectTimer m_timer;

	private GameObject m_timerVisual;

	public void OnEnable()
	{
		if (GameStateManager.Instance != null)
		{
			GameStateManager instance = GameStateManager.Instance;
			instance.OnSetupComplete = (Action)Delegate.Combine(instance.OnSetupComplete, new Action(onSetupComplete));
			GameStateManager instance2 = GameStateManager.Instance;
			instance2.OnEditorSetupComplete = (Action)Delegate.Combine(instance2.OnEditorSetupComplete, new Action(onSetupComplete));
		}
	}

	public void OnDisable()
	{
		if (GameStateManager.Instance != null)
		{
			GameStateManager instance = GameStateManager.Instance;
			instance.OnSetupComplete = (Action)Delegate.Remove(instance.OnSetupComplete, new Action(onSetupComplete));
			GameStateManager instance2 = GameStateManager.Instance;
			instance2.OnEditorSetupComplete = (Action)Delegate.Remove(instance2.OnEditorSetupComplete, new Action(onSetupComplete));
		}
	}

	private void onSetupComplete()
	{
		LevelObject component = base.gameObject.GetComponent<LevelObject>();
		Vector3 position = ((!(component == null)) ? base.gameObject.transform.TransformPoint(component.Offset) : base.gameObject.transform.position);
		m_timer = showTimer(position, TimerHUDScale, base.gameObject.transform.rotation, DelayTime, DelayMax, delegate
		{
			base.gameObject.SetActive(false);
		});
		if (DelayTime == 0f)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			m_timer.StartTimer();
		}
	}

	private ObjectTimer showTimer(Vector3 position, float scale, Quaternion rotation, float timeLimit, float timeMax, Action performAction = null)
	{
		m_timerVisual = UnityEngine.Object.Instantiate(TimerHUDPrefab, position, TimerHUDPrefab.transform.localRotation);
		m_timerVisual.transform.localScale = new Vector3(scale, scale, scale);
		m_timerVisual.gameObject.SetActive(true);
		m_timerVisual.gameObject.transform.parent = base.gameObject.transform;
		m_timerVisual.gameObject.transform.position = position;
		ObjectTimer componentInChildren = m_timerVisual.GetComponentInChildren<ObjectTimer>();
		componentInChildren.TimeLimit = timeLimit;
		componentInChildren.TimeMax = timeMax;
		componentInChildren.PerformActions = new List<Action>
		{
			performAction,
			delegate
			{
				UnityEngine.Object.Destroy(m_timerVisual);
			}
		};
		return componentInChildren;
	}
}
