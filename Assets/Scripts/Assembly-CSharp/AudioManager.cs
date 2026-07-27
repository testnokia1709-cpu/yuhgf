using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	public bool RemainBetweenScenes;

	private AudioSource m_music;

	private bool m_musicPaused;

	public int CurrentlyPlaying { get; private set; }

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

	public void Start()
	{
		PlayGameMusic(0);
	}

	public void Play(AudioSource source)
	{
		if (DataStore.Instance.GameSettings.MusicOn && source != null)
		{
			source.Play();
		}
	}

	public void PlayEffect(AudioSource source)
	{
		if (DataStore.Instance.GameSettings.MusicOn && source != null)
		{
			source.Play();
		}
	}

	public void PlayEffect(AudioClip clip, Vector3 position)
	{
		if (DataStore.Instance.GameSettings.MusicOn && clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, position);
		}
	}

	public void ChangeMusicPitch(float pitch)
	{
		if (m_music != null)
		{
			m_music.pitch = pitch;
		}
	}

	public void PlayMenuMusic()
	{
		m_music = AudioLibrary.Instance.MenuMusic;
		m_musicPaused = false;
	}

	public void PlayGameMusic(int index)
	{
		if (m_music != null && m_music.isPlaying)
		{
			m_music.Stop();
		}
		CurrentlyPlaying = index;
		m_music = AudioLibrary.Instance.GameMusic[index];
		m_musicPaused = false;
	}

	public void PauseMusic()
	{
		if (m_music != null)
		{
			m_music.Pause();
			m_musicPaused = true;
		}
	}

	public void ResumeMusic()
	{
		if (m_music != null)
		{
			m_music.UnPause();
			m_musicPaused = false;
		}
	}

	private void Update()
	{
		if (m_music != null && !m_musicPaused)
		{
			if (!m_music.isPlaying && DataStore.Instance.GameSettings.MusicOn)
			{
				m_music.Play();
			}
			else if (m_music.isPlaying && !DataStore.Instance.GameSettings.MusicOn)
			{
				m_music.Stop();
			}
		}
	}
}
