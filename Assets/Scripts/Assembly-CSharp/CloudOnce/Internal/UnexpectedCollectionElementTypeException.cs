using System;

namespace CloudOnce.Internal
{
	public class UnexpectedCollectionElementTypeException : Exception
	{
		public UnexpectedCollectionElementTypeException(string key, Type type)
			: base(string.Format("Unexpected type at index {0}, expected {1} ", key, type))
		{
		}
	}
}
