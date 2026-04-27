using System;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
	[NonSerialized] public int poolIndex;

	void Awake()
	{
	}

	void Start()
	{
		ResetResources();
		StartAI();
		StartAnimation();
	}

	void Update()
	{
		float deltaTime = Time.deltaTime;

		UpdateAI(deltaTime);
		UpdateAnimation(deltaTime);
		UpdateAudio(deltaTime);
	}

	public void ToggleActive(bool active)
	{
		gameObject.SetActive(active);
		animator.enabled = active;
		ActionStateEnum = ActionState.Idle;
	}
}