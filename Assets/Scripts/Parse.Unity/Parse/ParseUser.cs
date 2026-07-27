using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	[ParseClassName("_User")]
	public class ParseUser : ParseObject
	{
		private static readonly IDictionary<string, IParseAuthenticationProvider> authProviders = new Dictionary<string, IParseAuthenticationProvider>();

		private static readonly HashSet<string> readOnlyKeys = new HashSet<string> { "sessionToken", "isNew" };

		private static readonly object isRevocableSessionEnabledMutex = new object();

		private static bool isRevocableSessionEnabled;

		internal static IParseUserController UserController
		{
			get
			{
				return ParseCorePlugins.Instance.UserController;
			}
		}

		internal static IParseCurrentUserController CurrentUserController
		{
			get
			{
				return ParseCorePlugins.Instance.CurrentUserController;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				lock (mutex)
				{
					return SessionToken != null && CurrentUser != null && CurrentUser.ObjectId == base.ObjectId;
				}
			}
		}

		internal string SessionToken
		{
			get
			{
				if (base.State.ContainsKey("sessionToken"))
				{
					return base.State["sessionToken"] as string;
				}
				return null;
			}
		}

		internal static string CurrentSessionToken
		{
			get
			{
				Task<string> currentSessionTokenAsync = GetCurrentSessionTokenAsync();
				currentSessionTokenAsync.Wait();
				return currentSessionTokenAsync.Result;
			}
		}

		[ParseFieldName("username")]
		public string Username
		{
			get
			{
				return GetProperty<string>(null, "Username");
			}
			set
			{
				SetProperty(value, "Username");
			}
		}

		[ParseFieldName("password")]
		public string Password
		{
			private get
			{
				return GetProperty<string>(null, "Password");
			}
			set
			{
				SetProperty(value, "Password");
			}
		}

		[ParseFieldName("email")]
		public string Email
		{
			get
			{
				return GetProperty<string>(null, "Email");
			}
			set
			{
				SetProperty(value, "Email");
			}
		}

		public static ParseUser CurrentUser
		{
			get
			{
				Task<ParseUser> currentUserAsync = GetCurrentUserAsync();
				currentUserAsync.Wait();
				return currentUserAsync.Result;
			}
		}

		public static ParseQuery<ParseUser> Query
		{
			get
			{
				return new ParseQuery<ParseUser>();
			}
		}

		internal static bool IsRevocableSessionEnabled
		{
			get
			{
				lock (isRevocableSessionEnabledMutex)
				{
					return isRevocableSessionEnabled;
				}
			}
		}

		internal IDictionary<string, IDictionary<string, object>> AuthData
		{
			get
			{
				IDictionary<string, IDictionary<string, object>> result;
				if (TryGetValue<IDictionary<string, IDictionary<string, object>>>("authData", out result))
				{
					return result;
				}
				return null;
			}
			private set
			{
				this["authData"] = value;
			}
		}

		public override void Remove(string key)
		{
			if (key == "username")
			{
				throw new ArgumentException("Cannot remove the username key.");
			}
			base.Remove(key);
		}

		internal override bool IsKeyMutable(string key)
		{
			return !readOnlyKeys.Contains(key);
		}

		internal override void HandleSave(IObjectState serverState)
		{
			base.HandleSave(serverState);
			SynchronizeAllAuthData();
			CleanupAuthData();
			MutateState(delegate(MutableObjectState mutableClone)
			{
				mutableClone.ServerData.Remove("password");
			});
		}

		internal static Task<string> GetCurrentSessionTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return CurrentUserController.GetCurrentSessionTokenAsync(cancellationToken);
		}

		internal Task SetSessionTokenAsync(string newSessionToken)
		{
			return SetSessionTokenAsync(newSessionToken, CancellationToken.None);
		}

		internal Task SetSessionTokenAsync(string newSessionToken, CancellationToken cancellationToken)
		{
			MutateState(delegate(MutableObjectState mutableClone)
			{
				mutableClone.ServerData["sessionToken"] = newSessionToken;
			});
			return SaveCurrentUserAsync(this);
		}

		internal Task SignUpAsync(Task toAwait, CancellationToken cancellationToken)
		{
			if (AuthData == null)
			{
				if (string.IsNullOrEmpty(Username))
				{
					TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
					taskCompletionSource.TrySetException(new InvalidOperationException("Cannot sign up user with an empty name."));
					return taskCompletionSource.Task;
				}
				if (string.IsNullOrEmpty(Password))
				{
					TaskCompletionSource<object> taskCompletionSource2 = new TaskCompletionSource<object>();
					taskCompletionSource2.TrySetException(new InvalidOperationException("Cannot sign up user with an empty password."));
					return taskCompletionSource2.Task;
				}
			}
			if (!string.IsNullOrEmpty(base.ObjectId))
			{
				TaskCompletionSource<object> taskCompletionSource3 = new TaskCompletionSource<object>();
				taskCompletionSource3.TrySetException(new InvalidOperationException("Cannot sign up a user that already exists."));
				return taskCompletionSource3.Task;
			}
			IDictionary<string, IParseFieldOperation> currentOperations = StartSave();
			return toAwait.OnSuccess((Task _) => UserController.SignUpAsync(base.State, currentOperations, cancellationToken)).Unwrap().ContinueWith(delegate(Task<IObjectState> t)
			{
				if (t.IsFaulted || t.IsCanceled)
				{
					HandleFailedSave(currentOperations);
				}
				else
				{
					IObjectState result = t.Result;
					HandleSave(result);
				}
				return t;
			})
				.Unwrap()
				.OnSuccess((Task<IObjectState> _) => SaveCurrentUserAsync(this))
				.Unwrap();
		}

		public Task SignUpAsync()
		{
			return SignUpAsync(CancellationToken.None);
		}

		public Task SignUpAsync(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => SignUpAsync(toAwait, cancellationToken), cancellationToken);
		}

		public static Task<ParseUser> LogInAsync(string username, string password)
		{
			return LogInAsync(username, password, CancellationToken.None);
		}

		public static Task<ParseUser> LogInAsync(string username, string password, CancellationToken cancellationToken)
		{
			return UserController.LogInAsync(username, password, cancellationToken).OnSuccess(delegate(Task<IObjectState> t)
			{
				ParseUser user = ParseObject.FromState<ParseUser>(t.Result, "_User");
				return SaveCurrentUserAsync(user).OnSuccess((Task _) => user);
			}).Unwrap();
		}

		public static Task<ParseUser> BecomeAsync(string sessionToken)
		{
			return BecomeAsync(sessionToken, CancellationToken.None);
		}

		public static Task<ParseUser> BecomeAsync(string sessionToken, CancellationToken cancellationToken)
		{
			return UserController.GetUserAsync(sessionToken, cancellationToken).OnSuccess(delegate(Task<IObjectState> t)
			{
				ParseUser user = ParseObject.FromState<ParseUser>(t.Result, "_User");
				return SaveCurrentUserAsync(user).OnSuccess((Task _) => user);
			}).Unwrap();
		}

		internal override Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
		{
			lock (mutex)
			{
				if (base.ObjectId == null)
				{
					throw new InvalidOperationException("You must call SignUpAsync before calling SaveAsync.");
				}
				return base.SaveAsync(toAwait, cancellationToken).OnSuccess((Task _) => (!CurrentUserController.IsCurrent(this)) ? Task.FromResult(0) : SaveCurrentUserAsync(this)).Unwrap();
			}
		}

		internal override Task<ParseObject> FetchAsyncInternal(Task toAwait, CancellationToken cancellationToken)
		{
			return base.FetchAsyncInternal(toAwait, cancellationToken).OnSuccess((Task<ParseObject> t) => (!CurrentUserController.IsCurrent(this)) ? Task.FromResult(t.Result) : SaveCurrentUserAsync(this).OnSuccess((Task _) => t.Result)).Unwrap();
		}

		public static void LogOut()
		{
			LogOutAsync().Wait();
		}

		public static Task LogOutAsync()
		{
			return LogOutAsync(CancellationToken.None);
		}

		public static Task LogOutAsync(CancellationToken cancellationToken)
		{
			return GetCurrentUserAsync().OnSuccess(delegate(Task<ParseUser> t)
			{
				LogOutWithProviders();
				ParseUser user = t.Result;
				return (user == null) ? Task.FromResult(0) : user.taskQueue.Enqueue((Task toAwait) => user.LogOutAsync(toAwait, cancellationToken), cancellationToken);
			}).Unwrap();
		}

		internal Task LogOutAsync(Task toAwait, CancellationToken cancellationToken)
		{
			string sessionToken = SessionToken;
			if (sessionToken == null)
			{
				return Task.FromResult(0);
			}
			MutateState(delegate(MutableObjectState mutableClone)
			{
				mutableClone.ServerData.Remove("sessionToken");
			});
			Task task = ParseSession.RevokeAsync(sessionToken, cancellationToken);
			return Task.WhenAll(task, CurrentUserController.LogOutAsync(cancellationToken));
		}

		private static void LogOutWithProviders()
		{
			foreach (IParseAuthenticationProvider value in authProviders.Values)
			{
				value.Deauthenticate();
			}
		}

		internal static Task<ParseUser> GetCurrentUserAsync()
		{
			return GetCurrentUserAsync(CancellationToken.None);
		}

		internal static Task<ParseUser> GetCurrentUserAsync(CancellationToken cancellationToken)
		{
			return CurrentUserController.GetAsync(cancellationToken);
		}

		private static Task SaveCurrentUserAsync(ParseUser user)
		{
			return SaveCurrentUserAsync(user, CancellationToken.None);
		}

		private static Task SaveCurrentUserAsync(ParseUser user, CancellationToken cancellationToken)
		{
			return CurrentUserController.SetAsync(user, cancellationToken);
		}

		internal static void ClearInMemoryUser()
		{
			CurrentUserController.ClearFromMemory();
		}

		public static Task EnableRevocableSessionAsync()
		{
			return EnableRevocableSessionAsync(CancellationToken.None);
		}

		public static Task EnableRevocableSessionAsync(CancellationToken cancellationToken)
		{
			lock (isRevocableSessionEnabledMutex)
			{
				isRevocableSessionEnabled = true;
			}
			return GetCurrentUserAsync(cancellationToken).OnSuccess((Task<ParseUser> t) => t.Result.UpgradeToRevocableSessionAsync(cancellationToken));
		}

		internal static void DisableRevocableSession()
		{
			lock (isRevocableSessionEnabledMutex)
			{
				isRevocableSessionEnabled = false;
			}
		}

		internal Task UpgradeToRevocableSessionAsync()
		{
			return UpgradeToRevocableSessionAsync(CancellationToken.None);
		}

		internal Task UpgradeToRevocableSessionAsync(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => UpgradeToRevocableSessionAsync(toAwait, cancellationToken), cancellationToken);
		}

		internal Task UpgradeToRevocableSessionAsync(Task toAwait, CancellationToken cancellationToken)
		{
			string sessionToken = SessionToken;
			return toAwait.OnSuccess((Task _) => ParseSession.UpgradeToRevocableSessionAsync(sessionToken, cancellationToken)).Unwrap().OnSuccess((Task<string> t) => SetSessionTokenAsync(t.Result))
				.Unwrap();
		}

		public static Task RequestPasswordResetAsync(string email)
		{
			return RequestPasswordResetAsync(email, CancellationToken.None);
		}

		public static Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken)
		{
			return UserController.RequestPasswordResetAsync(email, cancellationToken);
		}

		private static IParseAuthenticationProvider GetProvider(string providerName)
		{
			IParseAuthenticationProvider value;
			if (authProviders.TryGetValue(providerName, out value))
			{
				return value;
			}
			return null;
		}

		private void CleanupAuthData()
		{
			lock (mutex)
			{
				if (!CurrentUserController.IsCurrent(this))
				{
					return;
				}
				IDictionary<string, IDictionary<string, object>> authData = AuthData;
				if (authData == null)
				{
					return;
				}
				foreach (KeyValuePair<string, IDictionary<string, object>> item in new Dictionary<string, IDictionary<string, object>>(authData))
				{
					if (item.Value == null)
					{
						authData.Remove(item.Key);
					}
				}
			}
		}

		private void SynchronizeAllAuthData()
		{
			lock (mutex)
			{
				IDictionary<string, IDictionary<string, object>> authData = AuthData;
				if (authData == null)
				{
					return;
				}
				foreach (KeyValuePair<string, IDictionary<string, object>> item in authData)
				{
					SynchronizeAuthData(GetProvider(item.Key));
				}
			}
		}

		private void SynchronizeAuthData(IParseAuthenticationProvider provider)
		{
			bool flag = false;
			lock (mutex)
			{
				IDictionary<string, IDictionary<string, object>> authData = AuthData;
				if (authData == null || provider == null)
				{
					return;
				}
				IDictionary<string, object> value;
				if (authData.TryGetValue(provider.AuthType, out value))
				{
					flag = provider.RestoreAuthentication(value);
				}
			}
			if (!flag)
			{
				UnlinkFromAsync(provider.AuthType, CancellationToken.None);
			}
		}

		internal Task LinkWithAsync(string authType, IDictionary<string, object> data, CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue(delegate
			{
				IDictionary<string, IDictionary<string, object>> dictionary = AuthData;
				if (dictionary == null)
				{
					IDictionary<string, IDictionary<string, object>> dictionary2 = (AuthData = new Dictionary<string, IDictionary<string, object>>());
					dictionary = dictionary2;
				}
				dictionary[authType] = data;
				AuthData = dictionary;
				return SaveAsync(cancellationToken);
			}, cancellationToken);
		}

		internal Task LinkWithAsync(string authType, CancellationToken cancellationToken)
		{
			return GetProvider(authType).AuthenticateAsync(cancellationToken).OnSuccess((Task<IDictionary<string, object>> t) => LinkWithAsync(authType, t.Result, cancellationToken)).Unwrap();
		}

		internal Task UnlinkFromAsync(string authType, CancellationToken cancellationToken)
		{
			return LinkWithAsync(authType, null, cancellationToken);
		}

		internal bool IsLinked(string authType)
		{
			lock (mutex)
			{
				return AuthData != null && AuthData.ContainsKey(authType) && AuthData[authType] != null;
			}
		}

		internal static Task<ParseUser> LogInWithAsync(string authType, IDictionary<string, object> data, CancellationToken cancellationToken)
		{
			ParseUser user = null;
			return UserController.LogInAsync(authType, data, cancellationToken).OnSuccess(delegate(Task<IObjectState> t)
			{
				user = ParseObject.FromState<ParseUser>(t.Result, "_User");
				lock (user.mutex)
				{
					if (user.AuthData == null)
					{
						user.AuthData = new Dictionary<string, IDictionary<string, object>>();
					}
					user.AuthData[authType] = data;
					user.SynchronizeAllAuthData();
				}
				return SaveCurrentUserAsync(user);
			}).Unwrap()
				.OnSuccess((Task t) => user);
		}

		internal static Task<ParseUser> LogInWithAsync(string authType, CancellationToken cancellationToken)
		{
			return GetProvider(authType).AuthenticateAsync(cancellationToken).OnSuccess((Task<IDictionary<string, object>> authData) => LogInWithAsync(authType, authData.Result, cancellationToken)).Unwrap();
		}

		internal static void RegisterProvider(IParseAuthenticationProvider provider)
		{
			authProviders[provider.AuthType] = provider;
			ParseUser currentUser = CurrentUser;
			if (currentUser != null)
			{
				currentUser.SynchronizeAuthData(provider);
			}
		}
	}
}
