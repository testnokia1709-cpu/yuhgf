using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Load Scene On Event", 1)]
	public class LoadSceneOnCloudOnceEvent : MonoBehaviour
	{
		private enum CloudOnceEvent
		{
			OnInitializeComplete = 0,
			OnCloudLoadComplete = 1,
			OnSignedInChanged = 2
		}

		[SerializeField]
		private CloudOnceEvent cloudOnceEvent;

		[SerializeField]
		private string sceneName;

		[SerializeField]
		private bool loadAdditive;

		[SerializeField]
		private bool loadAsync;

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
			LoadScene();
		}

		private void OnCloudLoadComplete(bool result)
		{
			LoadScene();
		}

		private void OnSignedInChanged(bool isSignedIn)
		{
			LoadScene();
		}

		private void LoadScene()
		{
			UnsubscribeEvents();
			if (string.IsNullOrEmpty(sceneName))
			{
				Debug.LogWarning("Scene name was empty, aborting load.");
			}
			else if (loadAdditive && loadAsync)
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			}
			else if (loadAdditive && !loadAsync)
			{
				SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			}
			else if (!loadAdditive && loadAsync)
			{
				SceneManager.LoadSceneAsync(sceneName);
			}
			else
			{
				SceneManager.LoadScene(sceneName);
			}
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
