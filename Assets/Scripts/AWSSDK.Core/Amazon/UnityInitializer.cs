using System;
using System.Diagnostics;
using System.Threading;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal;
using UnityEngine;

namespace Amazon
{
	public class UnityInitializer : MonoBehaviour
	{
		private static UnityInitializer _instance = null;

		private static object _lock = new object();

		private static Thread _mainThread;

		public static UnityInitializer Instance
		{
			get
			{
				return _instance;
			}
		}

		private UnityInitializer()
		{
		}

		public static void AttachToGameObject(GameObject gameObject)
		{
			if (gameObject != null)
			{
				gameObject.AddComponent<UnityInitializer>();
				UnityEngine.Debug.Log(string.Format("Attached unity initializer to {0}", gameObject.name));
				return;
			}
			throw new ArgumentNullException("gameObject");
		}

		public void Awake()
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = this;
					if (_mainThread == null || !_mainThread.Equals(Thread.CurrentThread))
					{
						_mainThread = Thread.CurrentThread;
					}
					AmazonHookedPlatformInfo.Instance.Init();
					UnityEngine.Object.DontDestroyOnLoad(this);
					TraceListener listener = new UnityDebugTraceListener("UnityDebug");
					AWSConfigs.AddTraceListener("Amazon", listener);
					_instance.gameObject.AddComponent<UnityMainThreadDispatcher>();
				}
				else if (this != _instance)
				{
					UnityEngine.Object.DestroyObject(this);
				}
			}
		}

		public static bool IsMainThread()
		{
			if (_mainThread == null)
			{
				throw new Exception("Main thread has not been set, is the AWSPrefab on the scene?");
			}
			return Thread.CurrentThread.Equals(_mainThread);
		}
	}
}
