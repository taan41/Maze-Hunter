public partial class Monster
{
	public enum ActionState : int
	{
		Idle = 0,
		Walk = 1,
		Run = 2,
		Attack = 3,
		Stagger = 4,
		Die = 5,
	}
}