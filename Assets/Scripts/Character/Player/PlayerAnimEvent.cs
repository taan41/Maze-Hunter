using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
	[SerializeField] Player player;

	public void OnRollEnd()
	{
		player.FinishRolling();
	}

	public void OnAttackEnd()
	{
		player.FinishAttacking();
	}

	public void SetAttackMoveMultiplier(float multiplier)
	{
		player.SetAttackMoveMultiplier(multiplier);
	}

	public void ToggleSwordHitbox(int enabled)
	{
		player.ToggleSwordHitbox(enabled != 0);
	}

	public void ToggleSwordEffects(int enabled)
	{
		player.ToggleSwordEffects(enabled != 0);
	}

	public void SwordGroundHit()
	{
		player.SwordGroundHit();
	}
}