using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
	public event Action	OnStateChanged;

	[Header("Interaction Settings")]
	public bool autoPerform = false;
	public float cooldown = 0f;
	public int stateCount = 1;
	public string[] prompts;

	protected int currentState = 0;
	protected bool isOnCooldown = false;

	public void Interact()
	{
		if (isOnCooldown) return;

		currentState++;
		if (currentState >= stateCount)
		{
			currentState = 0;
		}

		PerformAction(currentState);

		if (cooldown > 0f)
		{
			isOnCooldown = true;
			Invoke(nameof(ResetCooldown), cooldown);
		}

		if (stateCount > 1)
		{
			OnStateChanged?.Invoke();
		}
	}

	void ResetCooldown()
	{
		isOnCooldown = false;
	}

	protected abstract void PerformAction(int state);

	public string GetCurrentPrompt()
	{
		if (prompts != null)
		{
			if (currentState < prompts.Length)
			{
				return prompts[currentState];
			}
			else if (prompts.Length > 0)
			{
				return prompts[0];
			}
		}
		return "interact";
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Interactor interactor))
		{
			if (autoPerform)
			{
				interactor.PerformInteraction(this, true);
			}
			else
			{
				interactor.AddInteractable(this);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!autoPerform && other.TryGetComponent(out Interactor interactor))
		{
			interactor.RemoveInteractable(this);
		}
	}
}