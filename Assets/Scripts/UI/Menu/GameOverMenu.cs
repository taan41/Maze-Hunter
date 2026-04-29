using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MenuBase
{
	[Header("Score Info Components")]
	public TextMeshProUGUI time;
	public TextMeshProUGUI timeScore;
	public TextMeshProUGUI objective;
	public TextMeshProUGUI objectiveScore;
	public TextMeshProUGUI objectiveBonusScore;
	public TextMeshProUGUI kill;
	public TextMeshProUGUI killScore;
	public TextMeshProUGUI completeBonusScore;
	public TextMeshProUGUI totalScore;

	[Header("Buttons")]
	public Button mainMenuButton;

	GameProgress GameProgress => GameProgress.Instance;

	void Awake()
	{
		mainMenuButton.onClick.AddListener(Close);
	}

	public override void Close()
	{
		gameObject.SetActive(false);
		Time.timeScale = 1f;
		SceneManager.Instance.LoadScene(SceneManager.Instance.MainMenu);
	}

	public override void Open() => Open(true);

	public void Open(bool completedMission)
	{
		base.Open();
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		var scoreData = GameProgress.Instance.CalculateScore(completedMission);

		time.text = GameInfoUI.FormatTime(GameProgress.elapsedTime);
		timeScore.text = scoreData.time.ToString("F0");

		objective.text = $"{GameProgress.completedObjectiveCount} / {GameProgress.objectiveCount}";
		objectiveScore.text = scoreData.objective.ToString("F0");

		objectiveBonusScore.text = scoreData.objectiveBonus.ToString("F0");

		kill.text = GameProgress.killCount.ToString();
		killScore.text = scoreData.kill.ToString("F0");

		completeBonusScore.text = scoreData.completeBonus.ToString("F0");

		totalScore.text = scoreData.total.ToString("F0");

		HighscoreManager.Instance.SubmitScore((int)scoreData.total, GameProgress.elapsedTime);
	}
}