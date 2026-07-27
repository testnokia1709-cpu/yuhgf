using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parse.Internal
{
	internal class ParseCommand : HttpRequest
	{
		private const string revocableSessionTokenTrueValue = "1";

		public IDictionary<string, object> DataObject { get; private set; }

		public override Stream Data
		{
			get
			{
				if (base.Data != null)
				{
					return base.Data;
				}
				return base.Data = ((DataObject != null) ? new MemoryStream(Encoding.UTF8.GetBytes(Json.Encode(DataObject))) : null);
			}
			internal set
			{
				base.Data = value;
			}
		}

		public ParseCommand(string relativeUri, string method, string sessionToken = null, IList<KeyValuePair<string, string>> headers = null, IDictionary<string, object> data = null)
			: this(relativeUri, method, sessionToken, headers, null, (data != null) ? "application/json" : null)
		{
			DataObject = data;
		}

		public ParseCommand(string relativeUri, string method, string sessionToken = null, IList<KeyValuePair<string, string>> headers = null, Stream stream = null, string contentType = null)
		{
			base.Uri = new Uri(new Uri(ParseClient.CurrentConfiguration.Server), relativeUri);
			base.Method = method;
			Data = stream;
			base.Headers = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("X-Parse-Application-Id", ParseClient.CurrentConfiguration.ApplicationId),
				new KeyValuePair<string, string>("X-Parse-Client-Version", ParseClient.VersionString),
				new KeyValuePair<string, string>("X-Parse-Installation-Id", ParseClient.InstallationId.ToString())
			};
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					base.Headers.Add(header);
				}
			}
			if (!string.IsNullOrEmpty(ParseClient.PlatformHooks.AppBuildVersion))
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-App-Build-Version", ParseClient.PlatformHooks.AppBuildVersion));
			}
			if (!string.IsNullOrEmpty(ParseClient.PlatformHooks.AppDisplayVersion))
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-App-Display-Version", ParseClient.PlatformHooks.AppDisplayVersion));
			}
			if (!string.IsNullOrEmpty(ParseClient.PlatformHooks.OSVersion))
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-OS-Version", ParseClient.PlatformHooks.OSVersion));
			}
			if (!string.IsNullOrEmpty(ParseClient.MasterKey))
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-Master-Key", ParseClient.MasterKey));
			}
			else
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-Windows-Key", ParseClient.CurrentConfiguration.WindowsKey));
			}
			if (!string.IsNullOrEmpty(sessionToken))
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-Session-Token", sessionToken));
			}
			if (!string.IsNullOrEmpty(contentType))
			{
				base.Headers.Add(new KeyValuePair<string, string>("Content-Type", contentType));
			}
			if (ParseUser.IsRevocableSessionEnabled)
			{
				base.Headers.Add(new KeyValuePair<string, string>("X-Parse-Revocable-Session", "1"));
			}
		}
	}
}
