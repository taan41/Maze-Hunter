using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : Interactable
{
	[Header("Door Settings")]
	public bool openFull = true;

	Animator animator;
	int isOpenHash;
	int openFullHash;

	void Awake()
	{
		animator = GetComponent<Animator>();
		isOpenHash = Animator.StringToHash("isOpen");
		openFullHash = Animator.StringToHash("openFull");

		stateCount = 2;
		prompts = new string[] { "open", "close" };
	}

	void Start()
	{
		animator.SetBool(isOpenHash, false);
		if (openFull)
		{
			animator.SetBool(openFullHash, true);
		}
	}

	protected override void PerformAction(int state)
	{
		// state 0 = close, state 1 = open
		animator.SetBool(isOpenHash, state == 1);
	}
}