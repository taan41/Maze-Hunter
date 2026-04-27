using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PoolableAudioSource : MonoBehaviour
{
	public event Action<PoolableAudioSource> OnFinished;

	AudioSource audioSource;

	float timer = 0f;

	void Awake()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				OnFinished?.Invoke(this);
				gameObject.SetActive(false);
			}
		}
	}

	public void PlayOneShot(AudioClip clip, Vector3 pos, float volume, float pitch, float pitchRange)
	{
		audioSource.transform.position = pos;

		timer = clip.length;

		if (pitchRange > 0f)
		{
			audioSource.pitch = pitch + UnityEngine.Random.Range(-pitchRange, pitchRange);
		}
		else
		{
			audioSource.pitch = pitch;
		}
		audioSource.PlayOneShot(clip, volume);
	}
}