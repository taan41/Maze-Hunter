using System;
using UnityEngine;

public partial class Gun : MonoBehaviour
{
	public event Action<Monster, float> OnHitMonster;

	#region Fields
	[SerializeField] Transform bulletPoint;
	[SerializeField] GameEffect muzzleFlashFX;
	[SerializeField] GameEffect bulletTrailFX;
	[SerializeField] GameEffect bloodSpurtFX;
	[SerializeField] GameEffect bulletHoleFX;
	[SerializeField] string monsterLayerName = "Monster";
	[SerializeField] string environmentLayerName = "Environment";
	[SerializeField] float spreadMultiplier = 0.1f;

	Transform cameraTransform;

	int monsterLayer;
	int environmentLayer;
	LayerMask hitLayerMask;

	GlobalSettings GlobalSettings => GlobalSettingsManager.Instance.settings;
	bool EnableVFX => GlobalSettings.enableVFX;
	#endregion

	#region Methods
	void Start()
	{
		cameraTransform = CameraManager.Instance.cameraTransform;

		monsterLayer = LayerMask.NameToLayer(monsterLayerName);
		environmentLayer = LayerMask.NameToLayer(environmentLayerName);
		hitLayerMask = (1 << monsterLayer) | (1 << environmentLayer);
	}

	public void Shoot(float spread)
	{
		muzzleFlashFX.Play(EnableVFX && GlobalSettings.gunMuzzleFlashes);

		Vector3 rayDirection = Quaternion.Euler(
			UnityEngine.Random.Range(-spread, spread) * spreadMultiplier,
			UnityEngine.Random.Range(-spread, spread) * spreadMultiplier,
			0f
		) * cameraTransform.forward;

		bulletTrailFX.transform.rotation = Quaternion.LookRotation(rayDirection);
		bulletTrailFX.Play(EnableVFX);

		Ray ray = new(cameraTransform.position, rayDirection);

		if (Physics.Raycast(ray, out RaycastHit hit, 100f, hitLayerMask))
		{
			if (hit.collider.gameObject.layer == monsterLayer)
			{
				bloodSpurtFX.transform.SetPositionAndRotation(hit.point - rayDirection * 0.01f, Quaternion.LookRotation(hit.normal));
				bloodSpurtFX.Play(EnableVFX && GlobalSettings.bloodSplatter);

				MonsterHitbox monsterHitbox = hit.collider.GetComponent<MonsterHitbox>();
				OnHitMonster?.Invoke(monsterHitbox.monster, monsterHitbox.multiplier);
			}
			else
			{
				bulletHoleFX.transform.SetPositionAndRotation(hit.point - rayDirection * 0.01f, Quaternion.LookRotation(hit.normal));
				bulletHoleFX.Play(EnableVFX && GlobalSettings.bulletHoles);
			}
		}
	}
	#endregion
}