using UnityEngine;

public class Instatiator : MonoBehaviour
{
	[SerializeField] GameObject prefab;
	[SerializeField] Transform existingObject;
	[SerializeField] bool teleportPlayer = false;
	[SerializeField] Transform destination;

	void Start()
	{
		if (destination == null) return;

		if (prefab != null)
		{
			Instantiate(prefab, destination.position, destination.rotation);
		}

		if (existingObject != null)
		{
			existingObject.SetPositionAndRotation(destination.position, destination.rotation);
		}

		if (teleportPlayer)
		{
			Transform playerTransform = Player.Instance.transform;
			playerTransform.SetPositionAndRotation(destination.position, destination.rotation);
		}
	}
}