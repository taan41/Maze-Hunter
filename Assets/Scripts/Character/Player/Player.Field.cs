using UnityEngine;

public partial class Player
{
	[Header("=== REFERENCES ===")]
	public Animator animator;
	public CharacterController controller;
	public Interactor interactor;
	[SerializeField] Sword sword;
	[SerializeField] Gun gun;

	[Space]
	[Header("=== SETTINGS ===")]
	[Header("--- Weapons ---")]
	[SerializeField] SwordSettings swordSettings;
	[SerializeField] GunSettings gunSettings;

	[Header("--- Movement ---")]
	[SerializeField] float walkSpeed = 3f;
	[SerializeField] float sprintSpeed = 5f;
	[SerializeField] float forwardSpeedMultiplier = 1f;
	[SerializeField] float jumpSpeed = 8.0f;
	[SerializeField] float jumpMinDuration = 0.15f;
	[SerializeField] float rollSpeedMultiplier = 4f;
	[SerializeField] float rollSpeedDecay = 1f;
	[SerializeField] float rollInputTimeThreshold = 0.25f;

	[Header("--- Resources ---")]
	[SerializeField] float maxHealth = 100f;
	[SerializeField] float maxEnergy = 100f;
	[SerializeField] float maxAmmo = 20f;
	[SerializeField] float energyToHealthRatio = 5f;
	[SerializeField] float energyToHealthPerSec = 1f;
	[SerializeField] float healthCriticalThreshold = 0.3f;
	[SerializeField] float healthCriticalMultiplier = 2f;
	[SerializeField] float energyToAmmoRatio = 2f;
	[SerializeField] float energyToAmmoPerSec = 10f;

	[Header("--- Physics ---")]
	[SerializeField] float gravity = 20.0f;

	[Header("--- Animation ---")]
	[SerializeField] float walkAnimSpeed = 2f;
	[SerializeField] float sprintAnimSpeed = 3f;

	[Header("--- Camera ---")]
	[SerializeField] float rotateToCameraSpeed = 1000f;
	[SerializeField] float normalFOV = 60f;
	[SerializeField] float aimFOV = 27f;
	[SerializeField] bool idleFreeLook = true;
}