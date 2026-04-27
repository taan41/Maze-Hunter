using UnityEngine;
using static Player.ActionState;

public partial class Player
{
	#region Fields
	Vector3 inputDir = Vector3.zero;
	
	bool isAiming;

	float rollThresholdTimer = 0f;
	float queueAttackDelayTimer = 0f;
	float comboTimer = 0f;
	float empoweredTimer = 0f;
	float shootCooldownTimer = 0f;
	#endregion

	#region Unity Methods
	void StartControll()
	{
	}

	void UpdateControll(float deltaTime)
	{
		if (ActionStateEnum != Attack && comboTimer > 0f)
		{
			comboTimer -= deltaTime;
			if (comboTimer <= 0f)
			{
				SetCombo(0);
			}
		}

		if (queueAttackDelayTimer > 0f)
		{
			queueAttackDelayTimer -= deltaTime;
		}

		if (empoweredTimer > 0f)
		{
			empoweredTimer -= deltaTime;
			if (empoweredTimer <= 0f)
			{
				sword.ToggleGlow(false);
			}
		}

		if (shootCooldownTimer > 0f)
		{
			shootCooldownTimer -= deltaTime;
		}

		UpdateControllWeapon(deltaTime);
		UpdateControllMovement(deltaTime);
		UpdateControllOther(deltaTime);
	}

	void UpdateControllWeapon(float deltaTime)
	{
		isAiming = WeaponTypeEnum == WeaponType.Gun && AllowAttackGun() && Input.GetMouseButton(1);

		if (Input.GetMouseButton(0) && ActionStateEnum != Roll)
		{
			switch (WeaponTypeEnum)
			{
				case WeaponType.None:
					break;
				case WeaponType.Sword:
					if (!AllowAttackSword()) return;

					if (ActionStateEnum != Attack)
					{
						StartAttackSword();
					}
					else if (queueAttackDelayTimer <= 0f)
					{
						IsQueueingAttackBool = true;
					}
					break;
				case WeaponType.Gun:
					if (!AllowAttackGun()) return;
					if (shootCooldownTimer > 0f) return;
					StartAttackGun();
					break;
			}
		}

		if (ActionStateEnum == Attack || IsQueueingAttackBool) return;

		if (Input.GetKeyDown(KeyCode.R))
		{
			isReloading = true;
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			WeaponTypeEnum = WeaponType.Sword;
			ToggleWeaponObject(WeaponTypeEnum);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			WeaponTypeEnum = WeaponType.Gun;
			ToggleWeaponObject(WeaponTypeEnum);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			WeaponTypeEnum = WeaponType.None;
			ToggleWeaponObject(WeaponTypeEnum);
		}
	}

	void UpdateControllMovement(float deltaTime)
	{
		inputDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

		if (!controller.isGrounded || ActionStateEnum == Roll) return;

		if (Input.GetButtonDown("Jump") && !(WeaponTypeEnum == WeaponType.Sword && ActionStateEnum == Attack))
		{
			ActionStateEnum = Jump;
			return;
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			rollThresholdTimer = 0f;
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			if (rollThresholdTimer < rollInputTimeThreshold)
			{
				rollThresholdTimer = 0f;
				ActionStateEnum = Roll;
				ToggleSwordHitbox(false);
				return;
			}

			rollThresholdTimer = 0f;
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			rollThresholdTimer += deltaTime;
		}

		if (ActionStateEnum == Attack || IsQueueingAttackBool) return;

		if (rollThresholdTimer >= rollInputTimeThreshold && inputDir.sqrMagnitude >= 0.01f
			&& !(WeaponTypeEnum == WeaponType.Gun && Input.GetMouseButton(1)))
		{
			ActionStateEnum = Sprint;
			SetAnimDirectionFloats(inputDir.z, inputDir.x);
			return;
		}

		if (inputDir.sqrMagnitude < 0.01f)
		{
			if (ActionStateEnum != Idle)
			{
				ActionStateEnum = Idle;
			}
			return;
		}
		else
		{
			if (ActionStateEnum != Walk)
			{
				ActionStateEnum = Walk;
			}

			SetAnimDirectionFloats(inputDir.z, inputDir.x);
			return;
		}
	}

	void UpdateControllOther(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			interactor.PerformCurrentInteraction();
		}
	}
	#endregion

	#region Methods

	public void FinishRolling()
	{
		ActionStateEnum = Idle;
	}

	public void FinishAttacking()
	{
		comboTimer = swordSettings.comboResetTime;
		if (IsQueueingAttackBool)
		{
			StartAttackSword();
			return;
		}
		ActionStateEnum = Idle;
	}

	public void SetCombo(int combo)
	{
		if (combo > swordSettings.comboMax) combo = 1;
		ComboInt = combo;
	}

	public void ToggleSwordHitbox(bool enabled)
	{
		sword.ToggleHitbox(enabled);
	}

	public void ToggleSwordEffects(bool enabled)
	{
		if (enabled && ComboInt == swordSettings.comboMax)
		{
			empoweredTimer = swordSettings.empoweredDuration;
			sword.ToggleGlow(true);
		}
		sword.ToggleEffects(enabled);
	}

	#endregion
}