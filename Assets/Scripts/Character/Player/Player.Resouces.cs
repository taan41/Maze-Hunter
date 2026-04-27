using UnityEngine;

public partial class Player
{
	[Space]
	[Header("=== RESOURCES ===")]
	public CharacterResource health = new();
	public CharacterResource energy = new();
	public CharacterResource ammo = new();

	bool isReloading = false;
	float autoReloadWaitTimer = 0f;

	void StartResources()
	{
		ResetResources();

		health.OnDepleted += OnHealthDepleted;
	}

	void ResetResources()
	{
		health.Reset(maxHealth);
		energy.Reset(maxEnergy);
		ammo.Reset(maxAmmo);

		isReloading = false;
		autoReloadWaitTimer = 0f;
	}

	void UpdateResources(float deltaTime)
	{
		if (!health.isFull && !energy.isEmpty)
		{
			float energyCost = energyToHealthPerSec * deltaTime;
			if (health.normalized <= healthCriticalThreshold)
			{
				energyCost *= healthCriticalMultiplier;
			}
			if (energyCost > energy.current)
			{
				energyCost = energy.current;
			}
			energy.Change(-energyCost);
			health.Change(energyCost * energyToHealthRatio);
		}

		if (autoReloadWaitTimer > 0f)
		{
			autoReloadWaitTimer -= deltaTime;
			if (autoReloadWaitTimer <= 0f)
			{
				autoReloadWaitTimer = 0f;
				isReloading = true;
			}
		}

		if (!isReloading && ammo.isEmpty && !energy.isEmpty)
		{
			isReloading = true;
		}

		if (isReloading)
		{
			if (ammo.isFull || energy.isEmpty)
			{
				isReloading = false;
			}
			else
			{
				float energyCost = energyToAmmoPerSec * deltaTime;
				if (energyCost > energy.current)
				{
					energyCost = energy.current;
				}
				energy.Change(-energyCost);
				ammo.Change(energyCost * energyToAmmoRatio);
			}
		}
	}

	void OnHealthDepleted()
	{
		GameProgress.Instance.FinishGame(false);
	}
}