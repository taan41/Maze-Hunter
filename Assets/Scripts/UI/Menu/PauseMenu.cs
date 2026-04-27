using UnityEngine;
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
		SceneManager.Instance.LoadScene(SceneManager.Instance.MainMenu);
	}

	void OnQuitClicked()
	{
		Application.Quit();
	}

	public override void Open() => Open(true);

	public void Open(bool pauseTime)
	{
		gameObject.SetActive(true);
		if (pauseTime)
		{
			Time.timeScale = 0f;
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
			if (GameProgress.Instance != null)
			{
				GameProgress.Instance.SetTimeTracking(true);
			}
		}
	}
}