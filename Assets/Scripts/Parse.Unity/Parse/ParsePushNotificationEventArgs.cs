using System;
using System.Collections.Generic;

namespace Parse
{
	public class ParsePushNotificationEventArgs : EventArgs
	{
		public IDictionary<string, object> Payload { get; internal set; }

		public string StringPayload { get; internal set; }

		internal ParsePushNotificationEventArgs(IDictionary<string, object> payload)
		{
			Payload = payload;
			StringPayload = ParseClient.SerializeJsonString(payload);
		}

		internal ParsePushNotificationEventArgs(string stringPayload)
		{
			StringPayload = stringPayload;
			Payload = ParseClient.DeserializeJsonString(stringPayload);
		}
	}
}
