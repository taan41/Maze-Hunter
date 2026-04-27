using Unity.Cinemachine;
using UnityEngine;

public partial class Player
{
	Transform cameraTransform;
	CinemachineCamera cinemachineCamera;

	Crosshair crosshair;

	bool freeLooking;

	#region Methods
	void StartCamera()
	{
		cameraTransform = CameraManager.Instance.cameraTransform;
		cinemachineCamera = CameraManager.Instance.cinemachineCamera;

		crosshair = Crosshair.Instance;

		crosshair.ToggleGun(false);
	}

	void UpdateCamera(float deltaTime)
	{
		freeLooking = idleFreeLook && ActionStateEnum == ActionState.Idle && WeaponTypeEnum == WeaponType.None;

		if (!freeLooking)
		{
			RotateToCamera();
		}

		crosshair.ToggleGun(WeaponTypeEnum == WeaponType.Gun && AllowAttackGun());

		if (isAiming)
		{
			if (cinemachineCamera.Lens.FieldOfView >= aimFOV)
			{
				cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(
					cinemachineCamera.Lens.FieldOfView, 
					aimFOV, 
					deltaTime * 10f
				);
			}
			else
			{
				cinemachineCamera.Lens.FieldOfView = aimFOV;
			}

			crosshair.targetSpread = spread * normalFOV / aimFOV;
		}
		else
		{
			if (cinemachineCamera.Lens.FieldOfView <= normalFOV)
			{
				cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(
					cinemachineCamera.Lens.FieldOfView, 
					normalFOV, 
					deltaTime * 10f
				);
			}
			else
			{
				cinemachineCamera.Lens.FieldOfView = normalFOV;
			}

			crosshair.targetSpread = spread;
		}
	}

	void RotateToCamera()
	{
		Vector3 forward = cameraTransform.forward;
		forward.y = 0;

		if (forward.sqrMagnitude < 0.001f) return;

		Quaternion targetRotation = Quaternion.LookRotation(forward);
		transform.rotation = Quaternion.RotateTowards(
			transform.rotation, 
			targetRotation, 
			rotateToCameraSpeed * Time.deltaTime
		);
	}
	#endregion
}