using UnityEngine;

public class SceneManager : MonoBehaviour
{
	public static SceneManager Instance { get; private set; }

	[field: SerializeField] public string MainMenu { get; private set; } = "MainMenu";
	[field: SerializeField] public string GameScene { get; private set; } = "GameScene";

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	public void LoadScene(string sceneName)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
	}
}