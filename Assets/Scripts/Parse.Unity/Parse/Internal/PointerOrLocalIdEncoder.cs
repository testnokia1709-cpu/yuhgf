using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal class PointerOrLocalIdEncoder : ParseEncoder
	{
		private static readonly PointerOrLocalIdEncoder instance = new PointerOrLocalIdEncoder();

		public static PointerOrLocalIdEncoder Instance
		{
			get
			{
				return instance;
			}
		}

		protected override IDictionary<string, object> EncodeParseObject(ParseObject value)
		{
			if (value.ObjectId == null)
			{
				throw new ArgumentException("Cannot create a pointer to an object without an objectId");
			}
			return new Dictionary<string, object>
			{
				{ "__type", "Pointer" },
				{ "className", value.ClassName },
				{ "objectId", value.ObjectId }
			};
		}
	}
}
