using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance { get; private set; }

	public Transform cameraTransform;
	public CinemachineCamera cinemachineCamera;
	public Camera miminapCamera;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		if (cameraTransform == null)
		{
			cameraTransform = Camera.main.transform;
		}
	}
}