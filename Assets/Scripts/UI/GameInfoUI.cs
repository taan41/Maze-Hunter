using UnityEngine;
using TMPro;
using System.Text;

public class GameInfoUI : MonoBehaviour
{
	public TextMeshProUGUI timerText;
	public TextMeshProUGUI collectedItemCountText;
	public TextMeshProUGUI killCountText;

	GameProgress gameManager;
	int previousCollectedItemCount = -1;
	int previousKillCount = -1;

	void Start()
	{
		gameManager = GameProgress.Instance;
	}

	void Update()
	{
		timerText.text = FormatTime(gameManager.elapsedTime);

		if (gameManager.completedObjectiveCount != previousCollectedItemCount)
		{
			previousCollectedItemCount = gameManager.completedObjectiveCount;
			collectedItemCountText.text = $"{previousCollectedItemCount} / {gameManager.objectiveCount}";
		}

		if (gameManager.killCount != previousKillCount)
		{
			previousKillCount = gameManager.killCount;
			killCountText.text = previousKillCount.ToString();
		}
	}

	public static string FormatTime(float time)
	{
		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);
		int milliseconds = Mathf.FloorToInt(time * 1000f % 1000f);

		return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
	}
}