using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

	[SerializeField] AudioSource musicSource;
	[SerializeField] PoolableAudioSource soundEffectsSource;
	[SerializeField] AudioClip musicClip;

	public float MasterVolume => globalSettings.masterVolume;
	public float MusicVolume => globalSettings.musicVolume * globalSettings.masterVolume;
	public float SFXVolume => globalSettings.sfxVolume * globalSettings.masterVolume;

	GlobalSettings globalSettings;
	ObjectPool<PoolableAudioSource> soundEffectPool;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Multiple instances of AudioManager detected. Destroying duplicate.");
			Destroy(gameObject);
			return;
		}
	}

	void Start()
	{
		globalSettings = GlobalSettingsManager.Instance.settings;
		
		soundEffectPool = new ObjectPool<PoolableAudioSource>(CreateSoundEffectSource, 10);

		if (musicClip != null)
		{
			PlayMusic(musicClip);
		}

		ChangeAudioVolume();
		globalSettings.OnAudioSettingsChanged += ChangeAudioVolume;
	}

	void ChangeAudioVolume()
	{
		if (musicSource == null) return;
		
		musicSource.volume = MusicVolume;
	}

	public void PlayMusic(AudioClip clip, bool loop = true)
	{
		musicSource.clip = clip;
		musicSource.loop = loop;
		musicSource.volume = MusicVolume;
		musicSource.Play();
	}

	public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, float pitchRange = 0f)
	{
		PoolableAudioSource source = soundEffectPool.Get();
		source.transform.position = position;
		source.gameObject.SetActive(true);
		source.PlayOneShot(clip, position, volume * SFXVolume, pitch, pitchRange);
	}

	PoolableAudioSource CreateSoundEffectSource()
	{
		PoolableAudioSource source = Instantiate(soundEffectsSource, transform);
		source.gameObject.SetActive(false);
		source.OnFinished += ReturnSoundEffectSource;
		return source;
	}

	void ReturnSoundEffectSource(PoolableAudioSource source)
	{
		soundEffectPool.Return(source);
	}
}