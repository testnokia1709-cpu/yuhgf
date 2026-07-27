using UnityEngine;

public class DataStoreInitializer : MonoBehaviour
{
	private void Awake()
	{
		DataStore.Create();
	}
}
