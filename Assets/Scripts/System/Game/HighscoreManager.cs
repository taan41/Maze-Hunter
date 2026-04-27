using System;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
	[Serializable]
	public class ScoreEntry
	{
		public string date;
		public int score;
		public float time;
	}

	[Serializable]
	public class HighscoreData
	{
		public List<ScoreEntry> entries = new();
	}

	public static HighscoreManager Instance { get; private set; }

	public const int MAX_ENTRIES = 10;

	string SavePath => Application.persistentDataPath + "/highscores.json";

	public HighscoreData Data { get; private set; } = new HighscoreData();

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		Load();
	}
	
	void Save()
	{
		string json = JsonUtility.ToJson(Data, true);
		System.IO.File.WriteAllText(SavePath, json);
	}

	void Load()
	{
		if (!System.IO.File.Exists(SavePath)) return;

		string json = System.IO.File.ReadAllText(SavePath);
		Data = JsonUtility.FromJson<HighscoreData>(json);
	}

	public void SubmitScore(int score, float time)
	{
		ScoreEntry newEntry = new() { date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), score = score, time = time };

		Data.entries.Add(newEntry);
		Data.entries.Sort((a, b) => b.score.CompareTo(a.score));
		if (Data.entries.Count > MAX_ENTRIES)
		{
			Data.entries.RemoveRange(MAX_ENTRIES, Data.entries.Count - MAX_ENTRIES);
		}
		
		Save();
	}

	public void ClearScores()
	{
		Data.entries.Clear();
		Save();
	}
}