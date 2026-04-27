using UnityEngine;
using UnityEngine.AI;

public partial class Monster
{
	[Header("=== REFERENCES ===")]
	[SerializeField] Animator animator;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] Transform eyeTransform;
	[SerializeField] AudioSource audioSource;
	[SerializeField] MonsterHitbox attackHitbox;

	[Space]
	[Header("=== SETTINGS ===")]
	[Header("--- Movement ---")]
	[SerializeField] float walkSpeed = 0.5f;
	[SerializeField] float runSpeed = 1.5f;
	[SerializeField] float staggerDuration = 0.5f;

	[Header("--- Health ---")]
	[SerializeField] float maxHealth = 50f;
	[SerializeField] float maxHealthRandomDelta = 0f;

	[Header("--- Attack ---")]
	[SerializeField] float attackRange = 1f;
	[SerializeField] float attackDamage = 5f;
	[SerializeField] float attackCooldown = 3f;

	[Header("--- AI ---")]
	[SerializeField] float detectionRange = 10f;
	[SerializeField] float enragedChance = 0.2f;
	[SerializeField] float pathfindingInterval = 0.5f;
	[SerializeField] float sightCheckInterval = 1f;

	[Header("--- Animation ---")]
	[SerializeField] float staggerInterval = 0.2f;

	[Header("--- Audio ---")]
	[SerializeField] AudioClip[] idleSounds;
	[SerializeField] float idleSoundIntervalMin = 5f;
	[SerializeField] float idleSoundIntervalMax = 8f;
	[SerializeField] AudioClip[] chaseSounds;
	[SerializeField] float chaseSoundIntervalMin = 3f;
	[SerializeField] float chaseSoundIntervalMax = 6f;
	[SerializeField] AudioClip[] attackSounds;
	[SerializeField] float attackSoundChance = 0.5f;
	[SerializeField] AudioClip[] hurtSounds;
	[SerializeField] float hurtSoundChance = 0.5f;
}