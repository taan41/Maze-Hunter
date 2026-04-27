using System.Collections.Generic;
using UnityEngine;

public class HighscoreMenu : MenuBase
{
	public ScoreEntryDisplay scoreEntryPrefab;
	public RectTransform scoreEntryContainer;

	readonly List<ScoreEntryDisplay> scoreEntries = new();

	// void Awake()
	// {
	// 	// Pre-instantiate score entry displays to avoid runtime overhead
	// 	for (int i = 0; i < HighscoreManager.MAX_ENTRIES; i++)
	// 	{
	// 		var entryDisplay = Instantiate(scoreEntryPrefab, scoreEntryContainer);
	// 		scoreEntries.Add(entryDisplay);
	// 	}
	// }

	public override void Open()
	{
		var highscores = HighscoreManager.Instance.Data.entries;
		
		for (int i = 0; i < scoreEntries.Count; i++)
		{
			if (i < highscores.Count)
			{
				scoreEntries[i].SetScoreEntry(highscores[i], i + 1);
				scoreEntries[i].gameObject.SetActive(true);
			}
			else
			{
				scoreEntries[i].gameObject.SetActive(false);
			}
		}

		if (scoreEntries.Count < highscores.Count)
		{
			for (int i = scoreEntries.Count; i < highscores.Count; i++)
			{
				var entryDisplay = Instantiate(scoreEntryPrefab, scoreEntryContainer);
				entryDisplay.SetScoreEntry(highscores[i], i + 1);
				entryDisplay.gameObject.SetActive(true);
				scoreEntries.Add(entryDisplay);
			}
		}

		gameObject.SetActive(true);
	}

	public void ClearHighscores()
	{
		HighscoreManager.Instance.ClearScores();

		foreach (var entryDisplay in scoreEntries)
		{
			entryDisplay.gameObject.SetActive(false);
		}
	}
}