using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MenuBase
{
	[SerializeField] Button resumeButton;
	[SerializeField] Button settingsButton;
	[SerializeField] Button mainMenuButton;
	[SerializeField] Button quitButton;

	void Start()
	{
		resumeButton.onClick.AddListener(OnResumeClicked);
		settingsButton.onClick.AddListener(OnSettingsClicked);
		mainMenuButton.onClick.AddListener(OnMainMenuClicked);
		quitButton.onClick.AddListener(OnQuitClicked);
	}

	void OnResumeClicked()
	{
		Close();
	}

	void OnSettingsClicked()
	{
		Close(false);
		MenuManager.Instance.settingsMenuPanel.Open();
	}

	void OnMainMenuClicked()
	{
		Close(false);
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		SceneManager.Instance.LoadScene(SceneManager.Instance.MainMenu);
	}

	void OnQuitClicked()
	{
		Close(true);
		Application.Quit();
	}

	public override void Open() => Open(true);

	public void Open(bool pauseTime)
	{
		gameObject.SetActive(true);
		if (pauseTime)
		{
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			if (GameProgress.Instance != null)
			{
				GameProgress.Instance.SetTimeTracking(false);
			}
		}
	}

	public override void Close() => Close(true);

	public void Close(bool resumeTime)
	{
		gameObject.SetActive(false);
		if (resumeTime)
		{
			Time.timeScale = 1f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			if (GameProgress.Instance != null)
			{
				GameProgress.Instance.SetTimeTracking(true);
			}
		}
	}
}