using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour
{
	public static AudioLibrary Instance;

	public bool RemainBetweenScenes;

	public AudioSource MenuMusic;

	public List<AudioSource> GameMusic;

	public AudioSource EffectScreenshot;

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		if (RemainBetweenScenes)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
