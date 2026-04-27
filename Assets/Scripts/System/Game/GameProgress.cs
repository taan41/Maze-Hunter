using UnityEngine;

public class GameProgress : MonoBehaviour
{
	public static GameProgress Instance { get; private set; }

	public ScoreRule scoreRule;

	public float elapsedTime;
	public int objectiveCount;
	public int completedObjectiveCount;
	public int killCount;

	bool isTimeTracking = false;
	bool initializedTimeTracking = false;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		Reset();
	}

	void Update()
	{
		if (isTimeTracking)
		{
			elapsedTime += Time.deltaTime;
		}
	}

	public void Reset()
	{
		elapsedTime = 0f;
		isTimeTracking = false;
		initializedTimeTracking = false;

		objectiveCount = 0;
		completedObjectiveCount = 0;

		killCount = 0;
	}

	public void StartTimeTracking()
	{
		if (initializedTimeTracking) return;

		isTimeTracking = true;
		initializedTimeTracking = true;
	}

	public void SetTimeTracking(bool flag, bool resetTime = false)
	{
		if (!initializedTimeTracking) return;

		isTimeTracking = flag;

		if (resetTime)
		{
			elapsedTime = 0f;
			initializedTimeTracking = false;
		}
	}

	public (float time, float objective, float objectiveBonus, float kill, float completeBonus, float total) CalculateScore(bool completedMission)
	{
		bool allObjectivesCompleted = objectiveCount > 0 && completedObjectiveCount == objectiveCount;

		float timeScore = scoreRule.scoreTimeBase + elapsedTime * scoreRule.scoreTimePerSecond;
		float objectiveScore = completedObjectiveCount * scoreRule.scoreObjectivePerCompletion;
		float objectiveBonus = allObjectivesCompleted ? scoreRule.scoreObjectiveOnAllCompleted : 0f;
		float killScore = killCount * scoreRule.scoreKillPerKill;
		float completeBonus = completedMission && allObjectivesCompleted ? scoreRule.scoreMissionCompletionBonus : 0f;
		float totalScore = timeScore + objectiveScore + objectiveBonus + killScore + completeBonus;

		return (timeScore, objectiveScore, objectiveBonus, killScore, completeBonus, totalScore);
	}

	public void FinishGame(bool completedMission)
	{
		SetTimeTracking(false);
		MenuManager.Instance.OpenScoreMenu(completedMission);
	}
}