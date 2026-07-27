using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseCurrentUserController : IParseCurrentUserController, IParseObjectCurrentController<ParseUser>
	{
		private readonly object mutex = new object();

		private readonly TaskQueue taskQueue = new TaskQueue();

		private ParseUser currentUser;

		internal ParseUser CurrentUser
		{
			get
			{
				lock (mutex)
				{
					return currentUser;
				}
			}
			set
			{
				lock (mutex)
				{
					currentUser = value;
				}
			}
		}

		public Task SetAsync(ParseUser user, CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				if (user == null)
				{
					ParseClient.ApplicationSettings.Remove("CurrentUser");
				}
				else
				{
					IDictionary<string, object> dictionary = user.ServerDataToJSONObjectForSerialization();
					dictionary["objectId"] = user.ObjectId;
					if (user.CreatedAt.HasValue)
					{
						dictionary["createdAt"] = user.CreatedAt.Value.ToString(ParseClient.DateFormatStrings.First());
					}
					if (user.UpdatedAt.HasValue)
					{
						dictionary["updatedAt"] = user.UpdatedAt.Value.ToString(ParseClient.DateFormatStrings.First());
					}
					ParseClient.ApplicationSettings["CurrentUser"] = Json.Encode(dictionary);
				}
				CurrentUser = user;
			}), cancellationToken);
		}

		public Task<ParseUser> GetAsync(CancellationToken cancellationToken)
		{
			ParseUser parseUser;
			lock (mutex)
			{
				parseUser = CurrentUser;
			}
			if (parseUser != null)
			{
				return Task.FromResult(parseUser);
			}
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				object value;
				ParseClient.ApplicationSettings.TryGetValue("CurrentUser", out value);
				string text = value as string;
				ParseUser result = null;
				if (text != null)
				{
					IDictionary<string, object> data = Json.Parse(text) as IDictionary<string, object>;
					result = ParseObject.FromState<ParseUser>(ParseObjectCoder.Instance.Decode(data, ParseDecoder.Instance), "_User");
				}
				CurrentUser = result;
				return result;
			}), cancellationToken);
		}

		public Task<bool> ExistsAsync(CancellationToken cancellationToken)
		{
			if (CurrentUser != null)
			{
				return Task.FromResult(true);
			}
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith((Task t) => ParseClient.ApplicationSettings.ContainsKey("CurrentUser")), cancellationToken);
		}

		public bool IsCurrent(ParseUser user)
		{
			lock (mutex)
			{
				return CurrentUser == user;
			}
		}

		public void ClearFromMemory()
		{
			CurrentUser = null;
		}

		public void ClearFromDisk()
		{
			lock (mutex)
			{
				ClearFromMemory();
				ParseClient.ApplicationSettings.Remove("CurrentUser");
			}
		}

		public Task<string> GetCurrentSessionTokenAsync(CancellationToken cancellationToken)
		{
			return GetAsync(cancellationToken).OnSuccess(delegate(Task<ParseUser> t)
			{
				ParseUser result = t.Result;
				return (result != null) ? result.SessionToken : null;
			});
		}

		public Task LogOutAsync(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith((Task _) => GetAsync(cancellationToken)).Unwrap().OnSuccess(delegate
			{
				ClearFromDisk();
			}), cancellationToken);
		}
	}
}
