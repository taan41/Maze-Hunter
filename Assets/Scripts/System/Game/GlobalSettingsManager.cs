using UnityEngine;

public class GlobalSettingsManager : MonoBehaviour
{
	public static GlobalSettingsManager Instance { get; private set; }

	public GlobalSettings settings;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		settings = new GlobalSettings();
		settings.Restore();
	}
}