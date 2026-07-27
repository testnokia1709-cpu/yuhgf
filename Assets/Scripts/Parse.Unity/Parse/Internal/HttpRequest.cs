using System;
using System.Collections.Generic;
using System.IO;

namespace Parse.Internal
{
	internal class HttpRequest
	{
		public Uri Uri { get; internal set; }

		public IList<KeyValuePair<string, string>> Headers { get; internal set; }

		public virtual Stream Data { get; internal set; }

		public string Method { get; internal set; }
	}
}
