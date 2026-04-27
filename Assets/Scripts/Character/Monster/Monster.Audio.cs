using UnityEngine;

public partial class Monster
{
	float sfxTimer = 0f;

	void UpdateAudio(float deltaTime)
	{
		if (sfxTimer > 0f)
		{
			sfxTimer -= deltaTime;
			if (sfxTimer <= 0f)
			{
				switch (ActionStateEnum)
				{
					case ActionState.Idle:
						PlaySFXFromArray(idleSounds);
						sfxTimer = Random.Range(idleSoundIntervalMin, idleSoundIntervalMax);
						break;
					case ActionState.Walk:
					case ActionState.Run:
						PlaySFXFromArray(chaseSounds);
						sfxTimer = Random.Range(chaseSoundIntervalMin, chaseSoundIntervalMax);
						break;
					default:
						sfxTimer = 0f;
						break;
				}
			}
		}
	}

	void PlaySFXByState(ActionState state)
	{
		switch (state)
		{
			case ActionState.Idle:
				sfxTimer = Random.Range(idleSoundIntervalMin, idleSoundIntervalMax);
				break;
			case ActionState.Walk:
			case ActionState.Run:
				sfxTimer = Random.Range(chaseSoundIntervalMin, chaseSoundIntervalMax);
				break;
			case ActionState.Attack:
				sfxTimer = 0f;
				if (attackSounds.Length > 0 && Random.value < attackSoundChance)
				{
					PlaySFXFromArray(attackSounds);
				}
				break;
			case ActionState.Stagger:
				sfxTimer = 0f;
				if (hurtSounds.Length > 0 && Random.value < hurtSoundChance)
				{
					PlaySFXFromArray(hurtSounds);
				}
				break;
			default:
				break;
		}
	}

	void PlaySFXFromArray(AudioClip[] clips)
	{
		if (clips.Length > 0)
		{
			AudioClip clip = clips[Random.Range(0, clips.Length)];
			audioSource.PlayOneShot(clip);
		}
	}
}