using UnityEngine;

public class TimeStarter : Interactable
{
	void Awake()
	{
		autoPerform = true;
		stateCount = 1;
	}

	protected override void PerformAction(int state)
	{
		GameProgress.Instance.StartTimeTracking();
	}
}