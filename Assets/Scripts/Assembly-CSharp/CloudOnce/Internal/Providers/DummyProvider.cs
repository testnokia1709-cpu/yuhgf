using System;
using CloudOnce.Internal.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Providers
{
	public sealed class DummyProvider : CloudProviderBase<DummyProvider>
	{
		private CloudOnceEvents cloudOnceEvents;

		public override string PlayerID
		{
			get
			{
				return "DummyPlayerID";
			}
		}

		public override string PlayerDisplayName
		{
			get
			{
				return "DummyPlayerName";
			}
		}

		public override Texture2D PlayerImage
		{
			get
			{
				return Texture2D.whiteTexture;
			}
		}

		public override bool IsSignedIn
		{
			get
			{
				return false;
			}
		}

		public bool CloudSaveInitialized
		{
			get
			{
				return false;
			}
		}

		public override bool CloudSaveEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override ICloudStorageProvider Storage { get; protected set; }

		public override void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
		{
			cloudOnceEvents.RaiseOnInitializeComplete();
			cloudOnceEvents.RaiseOnPlayerImageDownloaded(Texture2D.whiteTexture);
			if (autoSignIn)
			{
				SignIn(autoCloudLoad);
			}
		}

		public override void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
		{
			CloudOnceUtils.SafeInvoke(callback, false);
			if (autoCloudLoad)
			{
				cloudOnceEvents.RaiseOnCloudLoadComplete(false);
			}
		}

		public override void SignOut()
		{
		}

		public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
		}

		public void InternalInit(CloudOnceEvents events)
		{
			cloudOnceEvents = events;
			Storage = new DummyStorageWrapper(events);
			base.ServiceName = "Dummy Provider";
		}
	}
}
