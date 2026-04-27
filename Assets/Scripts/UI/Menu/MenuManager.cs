using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance { get; private set; }

	[Header("Menu Panels")]
	public PauseMenu pauseMenuPanel;
	public SettingsMenu settingsMenuPanel;
	public GameOverMenu scoreMenuPanel;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();
		}
	}

	public void TogglePauseMenu()
	{
		if (!pauseMenuPanel.IsOpen)
		{
			if (settingsMenuPanel.IsOpen)
			{
				settingsMenuPanel.Close();
			}
			
			pauseMenuPanel.Open();
		}
		else
		{
			pauseMenuPanel.Close();
		}
	}

	public void OpenScoreMenu(bool completedMission)
	{
		if (pauseMenuPanel.IsOpen) pauseMenuPanel.Close(false);
		if (settingsMenuPanel.IsOpen) settingsMenuPanel.Close();
		scoreMenuPanel.Open(completedMission);
	}
}