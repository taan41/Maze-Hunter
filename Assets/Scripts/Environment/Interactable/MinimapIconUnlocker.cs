using UnityEngine;

public class MinimapIconUnlocker : Interactable
{
	[Header("Minimap Icon Unlocker Settings")]
	public GameObject minimapIcon;

	void Awake()
	{
		autoPerform = true;
		stateCount = 1;

	}

	void Start()
	{
		if (minimapIcon != null)
		{
			minimapIcon.SetActive(false);
		}
	}

	protected override void PerformAction(int state)
	{
		if (minimapIcon != null)
		{
			minimapIcon.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}