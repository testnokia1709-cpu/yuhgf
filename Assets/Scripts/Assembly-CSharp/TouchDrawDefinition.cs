using System.Collections.Generic;
using UnityEngine;

public class TouchDrawDefinition : MonoBehaviour
{
	public static TouchDrawDefinition Instance;

	public List<GameObject> ObjectPrefabs;

	public List<StringId> GoalStrings;

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(this);
	}
}
