using UnityEngine;

using static Monster.ActionState;

public partial class Monster
{
	[Header("=== REALTIME RESOURCES ===")]
	public CharacterResource health = new();

	void ResetResources()
	{
		health.Reset(maxHealth + maxHealthRandomDelta * Random.Range(-1f, 1f));
	}

	public void TakeDamage(float damage, Vector3 staggerDirection = default, float staggerSpeed = 0f)
	{
		if (ActionStateEnum == Die) return;

		health.Change(-damage);

		if (health.current <= 0f)
		{
			ActionStateEnum = Die;
			agent.isStopped = true;
			GameProgress.Instance.killCount++;
			return;
		}

		ActionStateEnum = Stagger;
		ToggleAttackHitbox(0);
		PlayStaggerAnimation();
		this.staggerDirection = staggerDirection;
		this.staggerSpeed = staggerSpeed;
		staggerCurrentSpeed = staggerSpeed;
		staggerTimer = staggerDuration;
	}
}