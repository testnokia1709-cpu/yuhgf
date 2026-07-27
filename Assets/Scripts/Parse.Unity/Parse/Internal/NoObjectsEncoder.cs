using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal class NoObjectsEncoder : ParseEncoder
	{
		private static readonly NoObjectsEncoder instance = new NoObjectsEncoder();

		public static NoObjectsEncoder Instance
		{
			get
			{
				return instance;
			}
		}

		protected override IDictionary<string, object> EncodeParseObject(ParseObject value)
		{
			throw new ArgumentException("ParseObjects not allowed here.");
		}
	}
}
