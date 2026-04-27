using UnityEngine;

using static Player.ActionState;

public partial class Player
{
	Vector3 moveDir = Vector3.zero;
	Vector3 rollDir = Vector3.zero;

	float verticalVelocity = 0f;
	float currentRollSpeedMultiplier = 1f;
	float attackMoveMultiplier = 0f;

	bool initJump = false;
	bool initRoll = false;

	float jumpTimer = 0f;

	void StartPhysics()
	{
	}

	void UpdatePhysics(float deltaTime)
	{
		if (jumpTimer > 0f)
		{
			jumpTimer -= deltaTime;
			if (jumpTimer <= 0f)
			{
				jumpTimer = 0f;
				initJump = false;
			}
		}

		if (ActionStateEnum == Attack && WeaponTypeEnum == WeaponType.Sword)
		{
			moveDir = transform.TransformDirection(Vector3.forward);
			moveDir *= walkSpeed * attackMoveMultiplier;
		}
		else switch (ActionStateEnum)
		{
			case Roll:
				if (!initRoll)
				{
					if (inputDir.sqrMagnitude > 0.01f)
					{
						rollDir = inputDir.normalized;
					}
					else
					{
						rollDir = Vector3.forward;
					}

					currentRollSpeedMultiplier = 1f;
					initRoll = true;
				}
			
				inputDir = rollDir;
				moveDir = transform.TransformDirection(inputDir);

				if (currentRollSpeedMultiplier > 0f)
				{
					currentRollSpeedMultiplier -= rollSpeedDecay * deltaTime;
					if (currentRollSpeedMultiplier < 0f)
					{
						currentRollSpeedMultiplier = 0f;
					}
				}

				moveDir *= walkSpeed * rollSpeedMultiplier * currentRollSpeedMultiplier;
				break;
			case Jump:
				if (controller.isGrounded)
				{
					if (!initJump)
					{
						verticalVelocity = jumpSpeed;
						jumpTimer = jumpMinDuration;
						initJump = true;
					}
					else if (jumpTimer <= 0f)
					{
						ActionStateEnum = Idle;
					}
				}
				goto default;
			case Sprint:
				moveDir = transform.TransformDirection(inputDir);
				moveDir *= sprintSpeed;
				if (inputDir.z > 0f && Mathf.Abs(inputDir.x) < 0.1f)
				{
					moveDir *= forwardSpeedMultiplier;
				}
				break;
			default:
				initRoll = false;
				moveDir = transform.TransformDirection(inputDir);
				moveDir *= walkSpeed;
				if (inputDir.z > 0f && Mathf.Abs(inputDir.x) < 0.1f)
				{
					moveDir *= forwardSpeedMultiplier;
				}
				break;
		}

		if (!controller.isGrounded)
		{
			verticalVelocity -= gravity * deltaTime;
		}
		else if (ActionStateEnum != Jump)
		{
			initJump = false;
			verticalVelocity = -0.1f;
		}

		moveDir.y = verticalVelocity;

		controller.Move(moveDir * deltaTime);
	}

	public void SetAttackMoveMultiplier(float multiplier)
	{
		attackMoveMultiplier = multiplier;
	}
}