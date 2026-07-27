using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Amazon.Runtime.Internal
{
	[Serializable]
	public class AutoConstructedDictionary<K, V> : Dictionary<K, V>
	{
		protected AutoConstructedDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
