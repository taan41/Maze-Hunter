using UnityEngine;

[CreateAssetMenu(fileName = "Score Rule", menuName = "Scriptables/ScoreRule")]
public class ScoreRule : ScriptableObject
{
	public float scoreTimeBase = 6000f;
	public float scoreTimePerSecond = -10f;
	public float scoreObjectivePerCompletion = 500f;
	public float scoreObjectiveOnAllCompleted = 1000f;
	public float scoreKillPerKill = 20f;
	public float scoreMissionCompletionBonus = 2000f;
}