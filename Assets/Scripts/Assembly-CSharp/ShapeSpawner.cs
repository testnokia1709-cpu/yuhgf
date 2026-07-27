using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
	public GameObject TemplateObject;

	public Transform SpawnLocation;

	public Transform SpawnParent;

	public float SpawnInterval;

	public bool InfiniteShapes;

	public int SpawnCount;

	public bool AddRigidBody2D;

	private int m_spawnedCount;

	private float m_timeSpawned;

	private void Start()
	{
		m_timeSpawned = Time.timeSinceLevelLoad;
		m_spawnedCount = 0;
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad - m_timeSpawned > SpawnInterval && (m_spawnedCount < SpawnCount || InfiniteShapes))
		{
			spawnObject();
			m_timeSpawned = Time.timeSinceLevelLoad;
			m_spawnedCount++;
		}
	}

	private void spawnObject()
	{
		GameObject gameObject = Object.Instantiate(TemplateObject, SpawnLocation.position, TemplateObject.transform.localRotation);
		gameObject.transform.parent = SpawnParent.transform;
		if (AddRigidBody2D)
		{
			gameObject.AddComponent<Rigidbody2D>();
		}
	}
}
