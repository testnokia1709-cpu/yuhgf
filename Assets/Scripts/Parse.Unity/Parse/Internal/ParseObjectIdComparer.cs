using System.Collections.Generic;

namespace Parse.Internal
{
	internal class ParseObjectIdComparer : IEqualityComparer<object>
	{
		bool IEqualityComparer<object>.Equals(object p1, object p2)
		{
			ParseObject parseObject = p1 as ParseObject;
			ParseObject parseObject2 = p2 as ParseObject;
			if (parseObject != null && parseObject2 != null)
			{
				return object.Equals(parseObject.ObjectId, parseObject2.ObjectId);
			}
			return object.Equals(p1, p2);
		}

		public int GetHashCode(object p)
		{
			ParseObject parseObject = p as ParseObject;
			if (parseObject != null)
			{
				return parseObject.ObjectId.GetHashCode();
			}
			return p.GetHashCode();
		}
	}
}
