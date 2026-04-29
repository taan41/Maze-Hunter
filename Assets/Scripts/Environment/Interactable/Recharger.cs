public class Recharger : Interactable
{
	void Awake()
	{
		prompts = new string[] { "recharge energy" };
	}

	protected override void PerformAction(int state)
	{
		Player.Instance.energy.Change(Player.Instance.energy.max);
	}
}