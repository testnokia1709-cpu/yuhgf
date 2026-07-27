using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Parse.Internal
{
	internal class IdentityEqualityComparer<T> : IEqualityComparer<T>
	{
		public bool Equals(T x, T y)
		{
			return (object)x == (object)y;
		}

		public int GetHashCode(T obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
}
