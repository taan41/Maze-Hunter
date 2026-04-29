using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MenuBase
{
	[SerializeField] Button startGameButton;

	void Awake()
	{
		startGameButton.onClick.AddListener(OnStartGameClicked);
	}

	void OnStartGameClicked()
	{
		SceneManager.Instance.LoadScene(SceneManager.Instance.GameScene);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}