using TMPro;
using UnityEngine;

public class ScoreEntryDisplay : MonoBehaviour
{
	public TextMeshProUGUI index;
	public TextMeshProUGUI score;
	public TextMeshProUGUI time;
	public TextMeshProUGUI date;

	public void SetScoreEntry(HighscoreManager.ScoreEntry entry, int index)
	{
		this.index.text = $"{index}.";
		score.text = entry.score.ToString();
		time.text = GameInfoUI.FormatTime(entry.time);
		date.text = entry.date;
	}
}