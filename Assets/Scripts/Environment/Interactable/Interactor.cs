using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactor : MonoBehaviour
{
	public enum FocusRule
	{
		Closest,
		First,
		Last,
	}

	public event Action OnInteractableChanged;

	[Header("Interactor Settings")]
	public FocusRule focusRule;

	public Interactable FocusedInteractable { get; private set; }

	readonly List<Interactable> interactablesInRange = new();

	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			if (interactable.autoPerform)
			{
				PerformInteraction(interactable, true);
			}
			else
			{
				AddInteractable(interactable);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			RemoveInteractable(interactable);
		}
	}

	public void PerformCurrentInteraction()
	{
		if (FocusedInteractable != null)
		{
			PerformInteraction(FocusedInteractable);
		}
	}

	public void PerformInteraction(Interactable interactable, bool bypassRangeCheck = false)
	{
		if (!bypassRangeCheck && !interactablesInRange.Contains(interactable)) return;

		interactable.Interact();
		if (interactable == null || !interactable.gameObject.activeSelf || interactable.useCount == 0)
		{
			RemoveInteractable(interactable);
		}
	}

	void UpdateCurrentInteractable()
	{
		if (interactablesInRange.Count == 0)
		{
			FocusedInteractable = null;
		}
		else
		{
			FocusedInteractable = focusRule switch
			{
				FocusRule.Closest => GetClosestInteractable(),
				FocusRule.First => interactablesInRange[0],
				FocusRule.Last => interactablesInRange[^1],
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
		interactablesInRange.Remove(interactable);
		UpdateCurrentInteractable();
	}
}