using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Providers
{
	public interface ICloudProvider
	{
		string ServiceName { get; }

		string PlayerID { get; }

		string PlayerDisplayName { get; }

		Texture2D PlayerImage { get; }

		bool IsSignedIn { get; }

		bool CloudSaveEnabled { get; set; }

		ICloudStorageProvider Storage { get; }

		void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true);

		void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null);

		void SignOut();

		void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);
	}
}
