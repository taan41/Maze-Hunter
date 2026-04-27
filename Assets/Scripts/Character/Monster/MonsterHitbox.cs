using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MonsterHitbox : MonoBehaviour
{
	public event Action<float> OnHitPlayer;
	
	public enum HitboxType
	{
		Body,
		Head,
		Attack,
	}

	public HitboxType hitboxType;
	public Monster monster;
	[Tooltip("Multiplier for damage dealt for Attack hitbox, or damage received for Body/Head hitbox")]
	[Min(0f)]
	public float multiplier = 1f;

	Collider hitboxCollider;

	void Awake()
	{
		hitboxCollider = GetComponent<Collider>();
		hitboxCollider.isTrigger = true;

		if (hitboxType == HitboxType.Attack)
		{
			gameObject.layer = LayerMask.NameToLayer("Attack");
		}
		else
		{
			gameObject.layer = LayerMask.NameToLayer("Monster");
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (hitboxType != HitboxType.Attack) return;

		if (other.CompareTag("Player"))
		{
			OnHitPlayer?.Invoke(multiplier);
		}
	}

	public void ToggleHitbox(bool enabled)
	{
		hitboxCollider.enabled = enabled;
	}

}