using UnityEngine;

public class Exit : Interactable
{
	void Awake()
	{
		prompts = new string[] { "finish mission" };
	}

	protected override void PerformAction(int _)
	{
		GameProgress.Instance.FinishGame(true);
	}
}