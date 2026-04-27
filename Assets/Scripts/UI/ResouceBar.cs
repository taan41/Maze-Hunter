using UnityEngine;
using UnityEngine.UI;

public class ResouceBar : MonoBehaviour
{
	public enum ResourceType { Health, Energy }

	[SerializeField] ResourceType resourceType;
	[SerializeField] Image fillImage;

	CharacterResource resource;

	void Start()
	{
		resource = resourceType switch
		{
			ResourceType.Health => Player.Instance.health,
			ResourceType.Energy => Player.Instance.energy,
			_ => null
		};
		resource.OnChanged += Refresh;
		Refresh();
	}

	void Refresh()
	{
		fillImage.fillAmount = resource.current / resource.max;
	}
}