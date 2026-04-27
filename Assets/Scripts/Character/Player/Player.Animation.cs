using UnityEngine;

public partial class Player
{
	const string ActionStateIntName = "actionStateInt";
	const string WeaponTypeIntName = "weaponTypeInt";
	const string ComboIntName = "comboInt";
	const string AttackTriggerName = "attackTrigger";
	const string IsQueueingAttackBoolName = "isQueueingAttackBool";
	const string MoveMultiplierFloatName = "moveMultiplierFloat";
	const string MoveFrontBackFloatName = "moveFrontBackFloat";
	const string MoveRightLeftFloatName = "moveRightLeftFloat";

	int ActionStateInt
	{
		get => animator.GetInteger(actionStateIntHash);
		set => animator.SetInteger(actionStateIntHash, value);
	}

	ActionState ActionStateEnum
	{
		get => (ActionState)ActionStateInt;
		set => ActionStateInt = (int)value;
	}

	int WeaponTypeInt
	{
		get => animator.GetInteger(weaponTypeHash);
		set => animator.SetInteger(weaponTypeHash, value);
	}

	WeaponType WeaponTypeEnum
	{
		get => (WeaponType)WeaponTypeInt;
		set => WeaponTypeInt = (int)value;
	}

	int ComboInt
	{
		get => animator.GetInteger(comboIntHash);
		set => animator.SetInteger(comboIntHash, value);
	}

	bool IsQueueingAttackBool
	{
		get => animator.GetBool(isQueueingAttackHash);
		set => animator.SetBool(isQueueingAttackHash, value);
	}

	float MoveMultiplierFloat
	{
		get => animator.GetFloat(moveMultiplierHash);
		set => animator.SetFloat(moveMultiplierHash, value);
	}

	float MoveFrontBackFloat
	{
		get => animator.GetFloat(moveFrontBackFloatHash);
		set => animator.SetFloat(moveFrontBackFloatHash, value);
	}

	float MoveRightLeftFloat
	{
		get => animator.GetFloat(moveRightLeftFloatHash);
		set => animator.SetFloat(moveRightLeftFloatHash, value);
	}

	int actionStateIntHash;
	int weaponTypeHash;
	int comboIntHash;
	int attackTriggerHash;
	int isQueueingAttackHash;
	int moveMultiplierHash;
	int moveFrontBackFloatHash;
	int moveRightLeftFloatHash;

	void AwakeAnimation()
	{
		actionStateIntHash = Animator.StringToHash(ActionStateIntName);
		weaponTypeHash = Animator.StringToHash(WeaponTypeIntName);
		comboIntHash = Animator.StringToHash(ComboIntName);
		attackTriggerHash = Animator.StringToHash(AttackTriggerName);
		isQueueingAttackHash = Animator.StringToHash(IsQueueingAttackBoolName);
		moveMultiplierHash = Animator.StringToHash(MoveMultiplierFloatName);
		moveFrontBackFloatHash = Animator.StringToHash(MoveFrontBackFloatName);
		moveRightLeftFloatHash = Animator.StringToHash(MoveRightLeftFloatName);
	}

	void StartAnimation()
	{
	}

	void UpdateAnimation(float deltaTime)
	{
		if (ActionStateEnum == ActionState.Walk)
		{
			MoveMultiplierFloat = walkSpeed / walkAnimSpeed;
		}
		else if (ActionStateEnum == ActionState.Sprint)
		{
			MoveMultiplierFloat = sprintSpeed / sprintAnimSpeed;
		}
		else
		{
			MoveMultiplierFloat = 1f;
		}
	}

	void SetAnimDirectionFloats(float frontBack, float rightLeft)
	{
		frontBack = Mathf.Clamp(frontBack, -1f, 1f);
		rightLeft = Mathf.Clamp(rightLeft, -1f, 1f);

		MoveFrontBackFloat = frontBack;
		MoveRightLeftFloat = rightLeft;
	}

	void SetTrigger(string triggerName)
	{
		animator.SetTrigger(triggerName);
	}

	void SetTrigger(int triggerHash)
	{
		animator.SetTrigger(triggerHash);
	}

	public void SetBoolQueueingAttack(bool value)
	{
		IsQueueingAttackBool = value;
	}
}