using System;
using CloudOnce.Internal.Providers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal
{
	public abstract class CloudProviderBase<T> : MonoBehaviour, ICloudProvider where T : Component
	{
		private static T s_instance;

		private float currentLoadTimer;

		public static T Instance
		{
			get
			{
				if (!object.ReferenceEquals(s_instance, null))
				{
					return s_instance;
				}
				UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(T));
				if (!object.ReferenceEquals(array, null) && array.Length > 0)
				{
					s_instance = array[0] as T;
					if (array.Length > 1)
					{
						for (int i = 1; i < array.Length; i++)
						{
							UnityEngine.Object.Destroy(array[i]);
						}
					}
				}
				if (!object.ReferenceEquals(s_instance, null))
				{
					return s_instance;
				}
				GameObject gameObject = new GameObject();
				gameObject.name = string.Format("NewTransient{0}Singleton", typeof(T));
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				GameObject gameObject2 = gameObject;
				s_instance = gameObject2.AddComponent(typeof(T)) as T;
				return s_instance;
			}
		}

		public string ServiceName { get; protected set; }

		public abstract string PlayerID { get; }

		public abstract string PlayerDisplayName { get; }

		public abstract Texture2D PlayerImage { get; }

		public abstract bool IsSignedIn { get; }

		public abstract bool CloudSaveEnabled { get; set; }

		public abstract ICloudStorageProvider Storage { get; protected set; }

		public abstract void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true);

		public abstract void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null);

		public abstract void SignOut();

		public abstract void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnOnDestroy()
		{
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			OnAwake();
		}

		private void Start()
		{
			currentLoadTimer = (float)Cloud.AutoLoadInterval;
		}

		private void Update()
		{
			if (Cloud.AutoLoadInterval != Interval.Disabled)
			{
				if (currentLoadTimer > 0f)
				{
					currentLoadTimer -= Time.deltaTime;
					return;
				}
				Cloud.Storage.Load();
				currentLoadTimer = (float)Cloud.AutoLoadInterval;
			}
		}

		private void OnDestroy()
		{
			s_instance = (T)null;
			OnOnDestroy();
		}
	}
}
