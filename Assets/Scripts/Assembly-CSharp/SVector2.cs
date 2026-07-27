using System;
using UnityEngine;

[Serializable]
public class SVector2 : ISerializationCallbackReceiver
{
	[NonSerialized]
	public float x;

	[NonSerialized]
	public float y;

	[SerializeField]
	private int u;

	[SerializeField]
	private int v;

	public SVector2(Vector2 vec)
	{
		x = vec.x;
		y = vec.y;
	}

	public SVector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public void OnBeforeSerialize()
	{
		u = Mathf.RoundToInt(x * 10f);
		v = Mathf.RoundToInt(y * 10f);
	}

	public void OnAfterDeserialize()
	{
		x = (float)u / 10f;
		y = (float)v / 10f;
	}

	public static implicit operator SVector2(Vector2 v)
	{
		return new SVector2(v);
	}

	public static implicit operator SVector2(Vector3 v)
	{
		return new SVector2(v);
	}

	public static implicit operator Vector2(SVector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector3(SVector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}
}
