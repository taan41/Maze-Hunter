using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public partial class Sword : MonoBehaviour
{
	public event Action<Monster> OnHitMonster;

	[SerializeField] BoxCollider hitbox;
	[SerializeField] GameEffect trailFX;
	[SerializeField] GameEffect electricTrailFX;
	[SerializeField] GameEffect electricGlowFX;
	[SerializeField] GameEffect onHitFX;
	[SerializeField] Transform skillActivePoint;
	[SerializeField] GameEffect skillFX;
	[SerializeField] ColliderEvent skillColliderEvent;
	[SerializeField] float skillScale = 1f;
	[SerializeField] float skillDuration = 0.5f;

	int monsterLayer;
	bool isGlowing;
	float skillTimer;

	void Awake()
	{
		if (hitbox == null) hitbox = GetComponent<BoxCollider>();
		monsterLayer = LayerMask.NameToLayer("Monster");

		skillColliderEvent.transform.localScale = Vector3.one * skillScale;
		skillColliderEvent.OnTriggerEnterEvent += OnGroundHitEvent;
	}

	void Start()
	{
		ToggleHitbox(false);
		ToggleGlow(false);
	}

	void Update()
	{
		if (skillTimer > 0f)
		{
			skillTimer -= Time.deltaTime;
			if (skillTimer <= 0f)
			{
				skillColliderEvent.ToggleCollider(false);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == monsterLayer)
		{
			if (other.TryGetComponent<MonsterHitbox>(out var monsterHitbox))
			{
				OnHitMonster?.Invoke(monsterHitbox.monster);

				Vector3 hitPoint = other.ClosestPoint(transform.position);
				onHitFX.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(transform.position - other.transform.position));
				onHitFX.Play();
			}
		}
	}

	public void ToggleHitbox(bool enabled)
	{
		if (enabled)
		{
			hitbox.enabled = true;
		}
		else
		{
			hitbox.enabled = false;
		}
	}

	public void ToggleEffects(bool enabled)
	{
		if (enabled)
		{
			if (isGlowing)
			{
				electricTrailFX.Play();
			}
			else
			{
				trailFX.Play();
			}
		}
		else
		{
			electricTrailFX.Stop();
			trailFX.Stop();
		}
	}

	public void ToggleGlow(bool enabled)
	{
		isGlowing = enabled;

		if (enabled)
		{
			electricGlowFX.Play();
		}
		else
		{
			electricGlowFX.Stop();
		}
	}

	public void GroundHit()
	{
		skillFX.transform.position = skillActivePoint.position;
		skillFX.Play();
		skillColliderEvent.ToggleCollider(true);
		skillTimer = skillDuration;
	}

	void OnGroundHitEvent(Collider other)
	{
		if (other.gameObject.layer == monsterLayer)
		{
			if (other.TryGetComponent<MonsterHitbox>(out var monsterHitbox))
			{
				OnHitMonster?.Invoke(monsterHitbox.monster);
			}
		}
	}
}