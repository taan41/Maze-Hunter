using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GameEffect : MonoBehaviour
{
	public enum EffectType
	{
		Once,
		Duration,
		Loop,
	}

	public event Action<GameEffect> OnDisable;
	
	[Header("--- Particle Settings ---")]
	[SerializeField] EffectType particleType;
	[SerializeField] ParticleSystem particle;
	[SerializeField] ParticleSystemStopBehavior stopBehavior;
	[SerializeField] float particleChance = 1f;

	[Space, Header("--- Audio Settings ---")]
	[SerializeField] EffectType audioType;
	[SerializeField] AudioClip audioClip;
	[SerializeField] AudioSource audioSource;
	[SerializeField] float audioChance = 1f;
	[SerializeField] float audioVolume = 1f;
	[SerializeField] float audioPitch = 1f;
	[SerializeField] float audioPitchRange = 0f;

	float timer = 0f;
	float duration = 1f;

	void Awake()
	{
		if (particle == null)
		{
			particle = GetComponent<ParticleSystem>();
		}

		duration = particle.main.duration;
		if (audioSource != null)
		{
			audioSource.playOnAwake = false;
		}
	}

	void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f && particleType == EffectType.Duration)
			{
				Stop();
			}
		}
	}

	public void Play()
	{
		if (particleType == EffectType.Duration)
		{
			timer = duration;
		}

		if (particleChance >= 1 || UnityEngine.Random.value <= particleChance)
		{
			particle.Play();
			var main = particle.main;
			main.loop = particleType == EffectType.Loop;
		}

		if (audioClip != null && (audioChance >= 1 || UnityEngine.Random.value <= audioChance))
		{
			if (audioSource != null)
			{
				audioSource.clip = audioClip;
				audioSource.loop = audioType == EffectType.Loop;
				audioSource.volume = audioVolume * AudioManager.Instance.SFXVolume;
				audioSource.pitch = audioPitch + UnityEngine.Random.Range(-audioPitchRange, audioPitchRange);
				audioSource.Play();
			}
			else
			{
				AudioManager.Instance.PlaySFX(audioClip, transform.position, audioVolume, audioPitch, audioPitchRange);
			}
		}
	}

	public void Stop()
	{
		particle.Stop(true, stopBehavior);
		if (audioSource != null && audioType != EffectType.Once)
		{
			audioSource.Stop();
		}
		OnDisable?.Invoke(this);
	}
}