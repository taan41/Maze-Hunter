using UnityEngine;

using static Monster.ActionState;

public partial class Monster
{
	Transform target;

	LayerMask sightLayerMask;
	int playerLayer;

	Vector3 staggerDirection;
	float staggerSpeed;
	float staggerCurrentSpeed;
	float staggerTimer;

	bool isEnraged;
	bool setAttackRotation;
	bool canSeeTarget;

	float attackRangeSqr;
	float detectionRangeSqr;

	float attackCooldownTimer;
	float pathfindingTimer;
	float sightCheckTimer;

	void StartAI()
	{
		target = Player.Instance.transform;

		sightLayerMask = LayerMask.GetMask("Environment", "Player");
		playerLayer = LayerMask.NameToLayer("Player");

		attackRangeSqr = attackRange * attackRange;
		detectionRangeSqr = detectionRange * detectionRange;

		attackHitbox.ToggleHitbox(false);
		attackHitbox.OnHitPlayer += DamagePlayer;
	}

	void UpdateAI(float deltaTime)
	{
		if (target == null || target.gameObject.activeInHierarchy == false)
		{
			gameObject.SetActive(false);
			return;
		}

		if (ActionStateEnum == Die) return;

		UpdateAITimers(deltaTime);

		UpdateAIDistanceLogic(deltaTime);
		
		UpdateAIActionLogic(deltaTime);
	}

	void UpdateAITimers(float deltaTime)
	{
		if (staggerTimer > 0f)
		{
			staggerTimer -= deltaTime;
			if (staggerTimer < 0f)
			{
				staggerTimer = 0f;
				ActionStateEnum = Idle;
			}
		}

		if (attackCooldownTimer > 0f)
		{
			attackCooldownTimer -= deltaTime;
			if (attackCooldownTimer < 0f)
			{
				attackCooldownTimer = 0f;
			}
		}

		if (pathfindingTimer > 0f)
		{
			pathfindingTimer -= deltaTime;
			if (pathfindingTimer < 0f)
			{
				pathfindingTimer = 0f;
			}
		}

		if (sightCheckTimer > 0f)
		{
			sightCheckTimer -= deltaTime;
			if (sightCheckTimer < 0f)
			{
				sightCheckTimer = 0f;
			}
		}
	}

	void UpdateAIDistanceLogic(float deltaTime)
	{
		if (staggerTimer > 0f)
		{
			ActionStateEnum = Stagger;
			return;
		}

		float targetDistanceSqr = (transform.position - target.position).sqrMagnitude;

		if (targetDistanceSqr <= attackRangeSqr)
		{
			ActionStateEnum = attackCooldownTimer > 0f ? Idle : Attack;
		}
		else if (targetDistanceSqr <= detectionRangeSqr)
		{
			if (ActionStateEnum == Idle || (ActionStateEnum == Attack && attackCooldownTimer > 0f))
			{
				isEnraged = isEnraged || Random.value < enragedChance;
				ActionStateEnum = isEnraged ? Run : Walk;
				pathfindingTimer = 0f;
				sightCheckTimer = 0f;
			}
		}
		else
		{
			if (ActionStateEnum != Idle)
			{
				if (sightCheckTimer <= 0f)
				{
					sightCheckTimer = sightCheckInterval;
					Ray sightRay = new(eyeTransform.position, (target.position - eyeTransform.position).normalized);
					if (Physics.Raycast(sightRay, out RaycastHit hit, detectionRange * 2f, sightLayerMask))
					{
						canSeeTarget = hit.collider.gameObject.layer == playerLayer;
					}
					else
					{
						canSeeTarget = false;
					}
				}
			}

			if (canSeeTarget)
			{
				ActionStateEnum = isEnraged ? Run : Walk;
			}
			else
			{
				ActionStateEnum = Idle;
			}
		}
	}

	void UpdateAIActionLogic(float deltaTime)
	{
		if (ActionStateEnum != Attack)
		{
			setAttackRotation = false;
		}

		switch (ActionStateEnum)
		{
			case Walk:
			case Run:
				agent.speed = ActionStateEnum == Walk ? walkSpeed : runSpeed;
				if (pathfindingTimer <= 0f)
				{
					pathfindingTimer = pathfindingInterval;
					agent.SetDestination(target.position);
				}
				break;
			case Attack:
				if (!setAttackRotation)
				{
					setAttackRotation = true;
					Vector3 directionToTarget = (target.position - transform.position).normalized;
					Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
					transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
				}
				agent.speed = 0f;
				break;
			case Stagger:
				agent.Move(deltaTime * staggerCurrentSpeed * staggerDirection);
				staggerCurrentSpeed = Mathf.InverseLerp(0f, staggerSpeed, staggerTimer / staggerDuration);
				break;
			default:
				agent.speed = 0f;
				break;
		}
	}

	void DamagePlayer(float multiplier = 1f)
	{
		Player.Instance.health.Change(-attackDamage * multiplier);
	}
}