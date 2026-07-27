using System.Threading;
using Amazon.Runtime.Internal;
using UnityEngine;

namespace Amazon.Util.Storage.Internal
{
	public class NetworkInfo
	{
		public static NetworkReachability Reachability
		{
			get
			{
				if (UnityInitializer.IsMainThread())
				{
					return Application.internetReachability;
				}
				NetworkReachability _networkReachability = NetworkReachability.NotReachable;
				AutoResetEvent asyncEvent = new AutoResetEvent(false);
				UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
				{
					_networkReachability = Application.internetReachability;
					asyncEvent.Set();
				});
				asyncEvent.WaitOne();
				return _networkReachability;
			}
		}
	}
}
