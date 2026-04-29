public class Objective : Interactable
{
	void Awake()
	{
		prompts = new string[] { "collect info" };
	}

	void Start()
	{
		GameProgress.Instance.objectiveCount++;
	}

	protected override void PerformAction(int state)
	{
		GameProgress.Instance.completedObjectiveCount++;
	}
}