using System;
using UnityEngine;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Deactivate On Event", 2)]
	public class DeactivateOnCloudOnceEvent : MonoBehaviour
	{
		private enum CloudOnceEvent
		{
			OnInitializeComplete = 0,
			OnCloudLoadComplete = 1,
			OnSignedInChanged = 2
		}

		[SerializeField]
		private CloudOnceEvent cloudOnceEvent;

		private void Awake()
		{
			switch (cloudOnceEvent)
			{
			case CloudOnceEvent.OnInitializeComplete:
				Cloud.OnInitializeComplete += OnInitializeComplete;
				break;
			case CloudOnceEvent.OnCloudLoadComplete:
				Cloud.OnCloudLoadComplete += OnCloudLoadComplete;
				break;
			case CloudOnceEvent.OnSignedInChanged:
				Cloud.OnSignedInChanged += OnSignedInChanged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void OnInitializeComplete()
		{
			UnsubscribeEvents();
			base.gameObject.SetActive(false);
		}

		private void OnCloudLoadComplete(bool result)
		{
			UnsubscribeEvents();
			base.gameObject.SetActive(false);
		}

		private void OnSignedInChanged(bool isSignedIn)
		{
			UnsubscribeEvents();
			base.gameObject.SetActive(false);
		}

		private void UnsubscribeEvents()
		{
			switch (cloudOnceEvent)
			{
			case CloudOnceEvent.OnInitializeComplete:
				Cloud.OnInitializeComplete -= OnInitializeComplete;
				break;
			case CloudOnceEvent.OnCloudLoadComplete:
				Cloud.OnCloudLoadComplete -= OnCloudLoadComplete;
				break;
			case CloudOnceEvent.OnSignedInChanged:
				Cloud.OnSignedInChanged -= OnSignedInChanged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
