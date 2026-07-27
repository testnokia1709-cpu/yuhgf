using System;
using Amazon.Util.Storage.Internal;
using UnityEngine;

namespace Amazon.Util.Internal.PlatformServices
{
	public class NetworkReachability : INetworkReachability
	{
		internal EventHandler<NetworkStatusEventArgs> mNetworkReachabilityChanged;

		internal static readonly object reachabilityChangedLock = new object();

		public NetworkStatus NetworkStatus
		{
			get
			{
				switch (NetworkInfo.Reachability)
				{
				case UnityEngine.NetworkReachability.ReachableViaCarrierDataNetwork:
					return NetworkStatus.ReachableViaCarrierDataNetwork;
				case UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork:
					return NetworkStatus.ReachableViaWiFiNetwork;
				default:
					return NetworkStatus.NotReachable;
				}
			}
		}

		public event EventHandler<NetworkStatusEventArgs> NetworkReachabilityChanged
		{
			add
			{
				lock (reachabilityChangedLock)
				{
					mNetworkReachabilityChanged = (EventHandler<NetworkStatusEventArgs>)Delegate.Combine(mNetworkReachabilityChanged, value);
				}
			}
			remove
			{
				lock (reachabilityChangedLock)
				{
					mNetworkReachabilityChanged = (EventHandler<NetworkStatusEventArgs>)Delegate.Remove(mNetworkReachabilityChanged, value);
				}
			}
		}

		internal void OnNetworkReachabilityChanged(NetworkStatus status)
		{
			EventHandler<NetworkStatusEventArgs> eventHandler = mNetworkReachabilityChanged;
			if (eventHandler != null)
			{
				eventHandler(null, new NetworkStatusEventArgs(status));
			}
		}
	}
}
