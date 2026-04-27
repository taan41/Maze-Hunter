using UnityEngine;

using static Monster.ActionState;

public partial class Monster
{
	ActionState ActionStateEnum
	{
		get => (ActionState)animator.GetInteger(actionStateHash);
		set
		{
			if (value == previousActionState) return;
			previousActionState = value;
			animator.SetInteger(actionStateHash, (int)value);
			SetVarian(value);
			PlaySFXByState(value);
		}
	}
	ActionState previousActionState;

	int actionStateHash;
	int idleVariantHash;
	int walkVariantHash;
	int runVariantHash;
	int attackVariantHash;
	int staggerVariantHash;
	int dieVariantHash;
	int staggerTriggerHash;

	float staggerWaitTime = 0f;

	void StartAnimation()
	{
		actionStateHash = Animator.StringToHash("ActionState");
		idleVariantHash = Animator.StringToHash("IdleVariant");
		walkVariantHash = Animator.StringToHash("WalkVariant");
		runVariantHash = Animator.StringToHash("RunVariant");
		attackVariantHash = Animator.StringToHash("AttackVariant");
		staggerVariantHash = Animator.StringToHash("StaggerVariant");
		dieVariantHash = Animator.StringToHash("DieVariant");
		staggerTriggerHash = Animator.StringToHash("Stagger");

		animator.SetInteger(actionStateHash, (int)Idle);
		SetVarian(Idle);
		PlaySFXByState(Idle);
	}

	void UpdateAnimation(float deltaTime)
	{
		if (staggerWaitTime > 0f)
		{
			staggerWaitTime -= deltaTime;
		}
	}

	void PlayStaggerAnimation()
	{
		if (staggerWaitTime <= 0f)
		{
			staggerWaitTime = staggerInterval;
			SetVarian(Stagger);
			SetTrigger(staggerTriggerHash);
		}
	}

	void SetVarian(ActionState state)
	{
		int variantHash = state switch
		{
			Idle => idleVariantHash,
			Walk => walkVariantHash,
			Run => runVariantHash,
			Attack => attackVariantHash,
			Stagger => staggerVariantHash,
			Die => dieVariantHash,
			_ => 0
		};
		animator.SetFloat(variantHash, Random.value);
	}

	void SetTrigger(int triggerHash) => animator.SetTrigger(triggerHash);

	/// <summary>
	/// Call this from animation event to toggle attack hitbox on and off.
	/// </summary>
	/// <param name="toggle">Pass 0 to disable, anything else to enable.</param>
	public void ToggleAttackHitbox(int toggle)
	{
		attackHitbox.ToggleHitbox(toggle != 0);
	}

	/// <summary>
	/// Call this at the end of attack animation to apply attack cooldown.
	/// </summary>
	public void FinishAttackAnimation()
	{
		attackCooldownTimer = attackCooldown;
	}

	/// <summary>
	/// Call this at the end of die animation.
	/// </summary>
	public void FinishDieAnimation()
	{
		ToggleActive(false);
		MonsterManager.Instance.ReturnMonster(this);
	}

	/// <summary>
	/// Call this from animation event to change action state.
	/// </summary>
	/// <param name="state">The new action state.</param>
	public void SetActionState(int state)
	{
		ActionStateEnum = (ActionState)state;
	}
}