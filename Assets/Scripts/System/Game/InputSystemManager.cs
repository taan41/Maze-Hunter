using UnityEngine;

public class InputSystemManager : MonoBehaviour
{
	public static InputSystemManager Instance { get; private set; }

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}
}