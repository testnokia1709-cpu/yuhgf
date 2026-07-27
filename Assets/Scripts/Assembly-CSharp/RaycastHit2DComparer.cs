using System.Collections.Generic;
using UnityEngine;

public class RaycastHit2DComparer : IComparer<RaycastHit2D>
{
	public int Compare(RaycastHit2D a, RaycastHit2D b)
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
