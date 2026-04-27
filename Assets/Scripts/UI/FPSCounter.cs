using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
	public TextMeshProUGUI fpsText;
	public TextMeshProUGUI lowestFPSText;

	float timer = 0;
	float frameCount = 0;
	float highestDelta = 0;

	void Update()
	{
		float deltaTime = Time.deltaTime;

		if (timer < 1f)
		{
			timer += deltaTime;
			frameCount++;

			if (deltaTime > highestDelta)
			{
				highestDelta = deltaTime;
			}
		}
		else
		{
			float fps = frameCount / timer;
			fpsText.text = $"FPS: {fps:F1}";

			lowestFPSText.text = $"Lowest FPS: {(highestDelta > 0 ? (timer / highestDelta).ToString("F1") : "N/A")}";

			timer -= 1f;
			frameCount = 0;
			highestDelta = 0;
		}
	}
}