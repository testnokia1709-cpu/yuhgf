using System.Collections.Generic;
using UnityEngine;

public class RaycastHitComparer : IComparer<RaycastHit>
{
	public int Compare(RaycastHit a, RaycastHit b)
	{
		if (a.distance > b.distance)
		{
			return 1;
		}
		if (a.distance < b.distance)
		{
			return -1;
		}
		return 0;
	}
}
