using System;

namespace Parse.Internal
{
	internal class FileState
	{
		private const string ParseFileSecureScheme = "https";

		private const string ParseFileSecureDomain = "files.parsetfss.com";

		public string Name { get; internal set; }

		public string MimeType { get; internal set; }

		public Uri Url { get; internal set; }

		public Uri SecureUrl
		{
			get
			{
				Uri url = Url;
				if (url != null && url.Host == "files.parsetfss.com")
				{
					return new UriBuilder(url)
					{
						Scheme = "https",
						Port = -1
					}.Uri;
				}
				return url;
			}
		}
	}
}
