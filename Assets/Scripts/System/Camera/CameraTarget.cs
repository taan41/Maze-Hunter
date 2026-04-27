using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(PositionConstraint))]
public class CameraTarget : MonoBehaviour
{
	PositionConstraint positionConstraint;
	Transform cameraTransform;

	void Awake()
	{
		positionConstraint = GetComponent<PositionConstraint>();
		positionConstraint.constraintActive = true;
	}

	void Start()
	{
		cameraTransform = CameraManager.Instance.cameraTransform;

		positionConstraint.SetSource(0, new ConstraintSource { sourceTransform = Player.Instance.transform, weight = 1f });
	}

	void Update()
	{
		Vector3 forward = cameraTransform.forward;
		forward.y = 0;

		if (forward.sqrMagnitude < 0.001f) return;

		transform.rotation = Quaternion.LookRotation(forward);
	}
}