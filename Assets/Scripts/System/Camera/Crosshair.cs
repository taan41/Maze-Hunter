using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
	public static Crosshair Instance { get; private set; }

	[SerializeField] RectTransform centerDot;
	[SerializeField] RectTransform gunCrosshair;
	[SerializeField] float spreadSpeed = 10f;
	
	[NonSerialized] public float targetSpread;

	bool isGunCrosshairActive;
	float currentSpread;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		isGunCrosshairActive = gunCrosshair.gameObject.activeSelf;
	}

	void Start()
	{
		if (gunCrosshair == null) gunCrosshair = transform.Find("Gun Crosshair").GetComponent<RectTransform>();
		if (centerDot == null) centerDot = transform.Find("Center Dot").GetComponent<RectTransform>();
	}

	void LateUpdate()
	{
		currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadSpeed);
		gunCrosshair.sizeDelta = new Vector2(currentSpread, currentSpread);
	}

	public void ResetSpread()
	{
		targetSpread = 0f;
	}

	public void ToggleGun(bool enable)
	{
		if (enable == isGunCrosshairActive) return;

		isGunCrosshairActive = enable;
		gunCrosshair.gameObject.SetActive(enable);
	}
}