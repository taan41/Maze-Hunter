using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactor : MonoBehaviour
{
	public enum CurrentInteractableRule
	{
		Closest,
		First,
		Last,
	}

	public event Action OnInteractableChanged;

	[Header("Interactor Settings")]
	public CurrentInteractableRule currentInteractableRule;

	public Interactable CurrentInteractable { get; private set; }

	readonly List<Interactable> interactablesInRange = new();

	public void PerformCurrentInteraction()
	{
		if (CurrentInteractable != null)
		{
			CurrentInteractable.Interact();
		}
	}

	public void PerformInteraction(Interactable interactable, bool bypassRangeCheck = false)
	{
		if (bypassRangeCheck || interactablesInRange.Contains(interactable))
		{
			interactable.Interact();
		}
	}

	void UpdateCurrentInteractable()
	{
		if (interactablesInRange.Count == 0)
		{
			CurrentInteractable = null;
		}
		else
		{
			CurrentInteractable = currentInteractableRule switch
			{
				CurrentInteractableRule.Closest => GetClosestInteractable(),
				CurrentInteractableRule.First => interactablesInRange[0],
				CurrentInteractableRule.Last => interactablesInRange[^1],
				_ => null,
			};
		}

		OnInteractableChanged?.Invoke();
	}

	Interactable GetClosestInteractable()
	{
		Interactable closestInteractable = null;
		float closestDistance = float.MaxValue;
		for (int i = 0; i < interactablesInRange.Count; i++)
		{
			float sqrDistance = (interactablesInRange[i].transform.position - transform.position).sqrMagnitude;
			if (sqrDistance < closestDistance)			{
				closestDistance = sqrDistance;
				closestInteractable = interactablesInRange[i];
			}
		}
		return closestInteractable;
	}

	public void AddInteractable(Interactable interactable)
	{
		if (interactablesInRange.Contains(interactable))
			return;
		
		interactablesInRange.Add(interactable);
		UpdateCurrentInteractable();
	}

	public void RemoveInteractable(Interactable interactable)
	{
		if (interactablesInRange.Contains(interactable))
		{
			interactablesInRange.Remove(interactable);
			UpdateCurrentInteractable();
		}
	}
}