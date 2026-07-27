using System;
using UnityEngine;

[Serializable]
public class TouchDrawObject : ISerializationCallbackReceiver
{
	[NonSerialized]
	public GameObject Object;

	[NonSerialized]
	public Collider2D Collider;

	[NonSerialized]
	public LevelObject LevelObject;

	[NonSerialized]
	public Vector2 Position;

	[NonSerialized]
	public float Rotation;

	[NonSerialized]
	public Vector2 Scale = Vector2.one;

	[NonSerialized]
	public string ObjectName;

	[NonSerialized]
	public float ZPos;

	[SerializeField]
	private SVector2 p;

	[SerializeField]
	private SVector2 s = new SVector2(Vector2.one);

	[SerializeField]
	private int r;

	[SerializeField]
	private string n;

	public void OnBeforeSerialize()
	{
		p = Position;
		s = Scale;
		r = Mathf.RoundToInt(Rotation);
		n = ObjectName;
	}

	public void OnAfterDeserialize()
	{
		Position = p;
		Scale = s;
		Rotation = r;
		ObjectName = n;
	}
}
