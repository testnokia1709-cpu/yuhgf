using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public class ParsePush
	{
		private object mutex;

		private IPushState state;

		internal static readonly SynchronizedEventHandler<ParsePushNotificationEventArgs> parsePushNotificationReceived = new SynchronizedEventHandler<ParsePushNotificationEventArgs>();

		public ParseQuery<ParseInstallation> Query
		{
			get
			{
				return state.Query;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					if (s.Channels != null && value != null && value.GetConstraint("channels") != null)
					{
						throw new InvalidOperationException("A push may not have both Channels and a Query with a channels constraint");
					}
					s.Query = value;
				});
			}
		}

		public IEnumerable<string> Channels
		{
			get
			{
				return state.Channels;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					if (value != null && s.Query != null && s.Query.GetConstraint("channels") != null)
					{
						throw new InvalidOperationException("A push may not have both Channels and a Query with a channels constraint");
					}
					s.Channels = value;
				});
			}
		}

		public DateTime? Expiration
		{
			get
			{
				return state.Expiration;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					if (s.ExpirationInterval.HasValue)
					{
						throw new InvalidOperationException("Cannot set Expiration after setting ExpirationInterval");
					}
					s.Expiration = value;
				});
			}
		}

		public DateTime? PushTime
		{
			get
			{
				return state.PushTime;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					DateTime now = DateTime.Now;
					if (value < now || value > now.AddDays(14.0))
					{
						throw new InvalidOperationException("Cannot set PushTime in the past or more than two weeks later than now");
					}
					s.PushTime = value;
				});
			}
		}

		public TimeSpan? ExpirationInterval
		{
			get
			{
				return state.ExpirationInterval;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					if (s.Expiration.HasValue)
					{
						throw new InvalidOperationException("Cannot set ExpirationInterval after setting Expiration");
					}
					s.ExpirationInterval = value;
				});
			}
		}

		public IDictionary<string, object> Data
		{
			get
			{
				return state.Data;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					if (s.Alert != null && value != null)
					{
						throw new InvalidOperationException("A push may not have both an Alert and Data");
					}
					s.Data = value;
				});
			}
		}

		public string Alert
		{
			get
			{
				return state.Alert;
			}
			set
			{
				MutateState(delegate(MutablePushState s)
				{
					if (s.Data != null && value != null)
					{
						throw new InvalidOperationException("A push may not have both an Alert and Data");
					}
					s.Alert = value;
				});
			}
		}

		private static IParsePushController PushController
		{
			get
			{
				return ParseCorePlugins.Instance.PushController;
			}
		}

		private static IParsePushChannelsController PushChannelsController
		{
			get
			{
				return ParseCorePlugins.Instance.PushChannelsController;
			}
		}

		public static event EventHandler<ParsePushNotificationEventArgs> ParsePushNotificationReceived
		{
			add
			{
				parsePushNotificationReceived.Add(value);
			}
			remove
			{
				parsePushNotificationReceived.Remove(value);
			}
		}

		public ParsePush()
		{
			mutex = new object();
			state = new MutablePushState
			{
				Query = ParseInstallation.Query
			};
		}

		internal IDictionary<string, object> Encode()
		{
			return ParsePushEncoder.Instance.Encode(state);
		}

		private void MutateState(Action<MutablePushState> func)
		{
			lock (mutex)
			{
				state = state.MutatedClone(func);
			}
		}

		public Task SendAsync()
		{
			return SendAsync(CancellationToken.None);
		}

		public Task SendAsync(CancellationToken cancellationToken)
		{
			return PushController.SendPushNotificationAsync(state, ParseUser.CurrentSessionToken, cancellationToken);
		}

		public static Task SendAlertAsync(string alert)
		{
			return new ParsePush
			{
				Alert = alert
			}.SendAsync();
		}

		public static Task SendAlertAsync(string alert, string channel)
		{
			return new ParsePush
			{
				Channels = new List<string> { channel },
				Alert = alert
			}.SendAsync();
		}

		public static Task SendAlertAsync(string alert, IEnumerable<string> channels)
		{
			return new ParsePush
			{
				Channels = channels,
				Alert = alert
			}.SendAsync();
		}

		public static Task SendAlertAsync(string alert, ParseQuery<ParseInstallation> query)
		{
			return new ParsePush
			{
				Query = query,
				Alert = alert
			}.SendAsync();
		}

		public static Task SendDataAsync(IDictionary<string, object> data)
		{
			return new ParsePush
			{
				Data = data
			}.SendAsync();
		}

		public static Task SendDataAsync(IDictionary<string, object> data, string channel)
		{
			return new ParsePush
			{
				Channels = new List<string> { channel },
				Data = data
			}.SendAsync();
		}

		public static Task SendDataAsync(IDictionary<string, object> data, IEnumerable<string> channels)
		{
			return new ParsePush
			{
				Channels = channels,
				Data = data
			}.SendAsync();
		}

		public static Task SendDataAsync(IDictionary<string, object> data, ParseQuery<ParseInstallation> query)
		{
			return new ParsePush
			{
				Query = query,
				Data = data
			}.SendAsync();
		}

		public static Task SubscribeAsync(string channel)
		{
			return SubscribeAsync(new List<string> { channel }, CancellationToken.None);
		}

		public static Task SubscribeAsync(string channel, CancellationToken cancellationToken)
		{
			return SubscribeAsync(new List<string> { channel }, cancellationToken);
		}

		public static Task SubscribeAsync(IEnumerable<string> channels)
		{
			return SubscribeAsync(channels, CancellationToken.None);
		}

		public static Task SubscribeAsync(IEnumerable<string> channels, CancellationToken cancellationToken)
		{
			return PushChannelsController.SubscribeAsync(channels, cancellationToken);
		}

		public static Task UnsubscribeAsync(string channel)
		{
			return UnsubscribeAsync(new List<string> { channel }, CancellationToken.None);
		}

		public static Task UnsubscribeAsync(string channel, CancellationToken cancellationToken)
		{
			return UnsubscribeAsync(new List<string> { channel }, cancellationToken);
		}

		public static Task UnsubscribeAsync(IEnumerable<string> channels)
		{
			return UnsubscribeAsync(channels, CancellationToken.None);
		}

		public static Task UnsubscribeAsync(IEnumerable<string> channels, CancellationToken cancellationToken)
		{
			return PushChannelsController.UnsubscribeAsync(channels, cancellationToken);
		}
	}
}
