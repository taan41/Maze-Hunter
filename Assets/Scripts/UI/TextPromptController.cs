using TMPro;
using UnityEngine;

public class TextPromptController : MonoBehaviour
{
	public TextMeshProUGUI interactPrompt;

	Interactor playerInteractor;
	Interactable currentInteractable;

	void Start()
	{
		playerInteractor = Player.Instance.interactor;
		playerInteractor.OnInteractableChanged += UpdateInteractPrompt;
		UpdateInteractPrompt();
	}

	void UpdateInteractPrompt()
	{
		if (playerInteractor.FocusedInteractable != null)
		{
			currentInteractable = playerInteractor.FocusedInteractable;

			currentInteractable.OnStateChanged += UpdateInteractPromptText;
			UpdateInteractPromptText();
			
			interactPrompt.gameObject.SetActive(true);
		}
		else
		{
			if (currentInteractable != null)
			{
				currentInteractable.OnStateChanged -= UpdateInteractPromptText;
				currentInteractable = null;
			}

			interactPrompt.gameObject.SetActive(false);
		}
	}

	void UpdateInteractPromptText()
	{
		if (currentInteractable != null)
		{
			interactPrompt.text = "Press E to " + currentInteractable.GetCurrentPrompt();
		}
		else
		{
			interactPrompt.text = "";
		}
	}
}