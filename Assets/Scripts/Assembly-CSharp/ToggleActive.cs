using UnityEngine;

public class ToggleActive : MonoBehaviour
{
	public float Delay;

	public GameObject TargetObject;

	public bool RunOnce;

	private float m_time;

	public void Awake()
	{
	}

	public void Start()
	{
		m_time = Time.unscaledTime;
	}

	public void Update()
	{
		if (Time.unscaledTime - m_time > Delay)
		{
			TargetObject.SetActive(!TargetObject.activeSelf);
			m_time = Time.unscaledTime;
			if (RunOnce)
			{
				base.enabled = false;
			}
		}
	}
}
