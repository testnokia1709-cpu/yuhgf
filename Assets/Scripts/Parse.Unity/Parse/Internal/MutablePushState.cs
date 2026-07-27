using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal class MutablePushState : IPushState
	{
		public ParseQuery<ParseInstallation> Query { get; set; }

		public IEnumerable<string> Channels { get; set; }

		public DateTime? Expiration { get; set; }

		public TimeSpan? ExpirationInterval { get; set; }

		public DateTime? PushTime { get; set; }

		public IDictionary<string, object> Data { get; set; }

		public string Alert { get; set; }

		public IPushState MutatedClone(Action<MutablePushState> func)
		{
			MutablePushState mutablePushState = MutableClone();
			func(mutablePushState);
			return mutablePushState;
		}

		protected virtual MutablePushState MutableClone()
		{
			return new MutablePushState
			{
				Query = Query,
				Channels = ((Channels == null) ? null : new List<string>(Channels)),
				Expiration = Expiration,
				ExpirationInterval = ExpirationInterval,
				PushTime = PushTime,
				Data = ((Data == null) ? null : new Dictionary<string, object>(Data)),
				Alert = Alert
			};
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is MutablePushState))
			{
				return false;
			}
			MutablePushState mutablePushState = obj as MutablePushState;
			if (object.Equals(Query, mutablePushState.Query) && Channels.CollectionsEqual(mutablePushState.Channels) && object.Equals(Expiration, mutablePushState.Expiration) && object.Equals(ExpirationInterval, mutablePushState.ExpirationInterval) && object.Equals(PushTime, mutablePushState.PushTime) && Data.CollectionsEqual(mutablePushState.Data))
			{
				return object.Equals(Alert, mutablePushState.Alert);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
