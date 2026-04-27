using System.Collections.Generic;

using static Player.ActionState;
public partial class Player
{
	Dictionary<Monster, float> ignoredMonstersTimers = new();
	List<Monster> ignoredMonstersSnapshot = new();

	float spread = 0f;
	float shootSpread = 0f;

	void StartWeapon()
	{
		ToggleWeaponObject(WeaponTypeEnum);

		sword.OnHitMonster += OnSwordHit;
		gun.OnHitMonster += OnGunHit;
	}

	void UpdateWeapon(float deltaTime)
	{
		if (shootSpread > 0f)
		{
			shootSpread -= gunSettings.spread.shootDecrease * deltaTime;
			if (shootSpread < 0f) shootSpread = 0f;
		}

		RefreshSpread();

		if (ignoredMonstersTimers.Count == 0) return;

		ignoredMonstersSnapshot.Clear();

		foreach (var monster in ignoredMonstersTimers.Keys)
		{
			ignoredMonstersSnapshot.Add(monster);
		}

		for (int i = 0; i < ignoredMonstersSnapshot.Count; i++)
		{
			Monster monster = ignoredMonstersSnapshot[i];

			if (!ignoredMonstersTimers.TryGetValue(monster, out float remainingTime)) continue;

			remainingTime -= deltaTime;
			if (remainingTime <= 0f)
			{
				ignoredMonstersTimers.Remove(monster);
				continue;
			}

			ignoredMonstersTimers[monster] = remainingTime;
		}

		ignoredMonstersSnapshot.Clear();
	}

	void ToggleWeaponObject(WeaponType type)
	{
		sword.gameObject.SetActive(type == WeaponType.Sword);
		gun.gameObject.SetActive(type == WeaponType.Gun);
	}

	void StartAttackSword()
	{
		ActionStateEnum = Attack;
		SetCombo(ComboInt + 1);
		SetTrigger(attackTriggerHash);
		IsQueueingAttackBool = false;
		queueAttackDelayTimer = swordSettings.queueAttackInputDelay;
	}

	void StartAttackGun()
	{
		SetTrigger(attackTriggerHash);
		shootCooldownTimer = gunSettings.shootCooldown;
		autoReloadWaitTimer = gunSettings.autoReloadWaitTime;
		
		gun.Shoot(spread);
		ammo.Change(-1);

		shootSpread += gunSettings.spread.shootDelta;
		if (shootSpread > gunSettings.spread.shootMax)
		{
			shootSpread = gunSettings.spread.shootMax;
		}

		RefreshSpread();
	}

	void OnSwordHit(Monster monster)
	{
		if (ignoredMonstersTimers.ContainsKey(monster)) return;

		if (empoweredTimer > 0f)
		{
			monster.TakeDamage(swordSettings.damage * swordSettings.empoweredMultiplier, transform.forward, swordSettings.staggerSpeed * swordSettings.empoweredMultiplier);
			energy.Change(swordSettings.energyOnHit * swordSettings.empoweredMultiplier);
		}
		else
		{
			monster.TakeDamage(swordSettings.damage, transform.forward, swordSettings.staggerSpeed);
			energy.Change(swordSettings.energyOnHit);
		}

		ignoredMonstersTimers[monster] = swordSettings.hitIgnoreDuration;
	}

	void OnGunHit(Monster monster, float multiplier)
	{
		monster.TakeDamage(gunSettings.damage * multiplier, transform.forward, gunSettings.staggerSpeed * multiplier);
	}

	void RefreshSpread()
	{
		if (WeaponTypeEnum != WeaponType.Gun)
		{
			spread = 0f;
			shootSpread = 0f;
			return;
		}

		spread = gunSettings.spread._base;

		if (ActionStateEnum == Walk)
		{
			spread += gunSettings.spread.walkDelta;
		}
		else if (ActionStateEnum == Jump)
		{
			spread += gunSettings.spread.jumpDelta;
		}

		spread += shootSpread;
	}

	bool AllowAttackSword()
	{
		return controller.isGrounded && ActionStateEnum != Jump && ActionStateEnum != Roll;
	}

	bool AllowAttackGun()
	{
		return !ammo.isEmpty && (ActionStateEnum == Idle || ActionStateEnum == Walk || ActionStateEnum == Jump);
	}

	public void SwordGroundHit()
	{
		sword.GroundHit();
	}
}