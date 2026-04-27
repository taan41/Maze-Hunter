using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
	[SerializeField] bool alignWithCamera = true;
	[SerializeField] bool hideFull = false;
	[SerializeField] bool hideEmpty = false;
	[SerializeField] Canvas canvas;
	[SerializeField] Image fillImage;
	[SerializeField] Monster monster;

	Transform cameraTransform;
	CharacterResource monsterHealth;

	void Start()
	{
		cameraTransform = CameraManager.Instance.cameraTransform;

		monsterHealth = monster.health;
		monsterHealth.OnChanged += Refresh;
		Refresh();
	}

	void LateUpdate()
	{
		if (alignWithCamera)
		{
			AlignWithCamera();
		}
	}

	void AlignWithCamera()
	{
		transform.forward = cameraTransform.forward;
	}

	void Refresh()
	{
		if (hideFull && monsterHealth.isFull)
		{
			canvas.enabled = false;
		}
		else if (hideEmpty && monsterHealth.current <= 0f)
		{
			canvas.enabled = false;
		}
		else
		{
			canvas.enabled = true;
		}

		fillImage.fillAmount = monsterHealth.current / monsterHealth.max;
	}
}