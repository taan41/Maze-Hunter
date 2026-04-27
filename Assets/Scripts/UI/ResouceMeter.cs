using System.Text;
using TMPro;
using UnityEngine;

public class ResourceMeter : MonoBehaviour
{
	public enum ResourceType { Health, Energy , Ammo }
	public enum NumberFormat { Integer, Decimal }

	[SerializeField] ResourceType resourceType;
	[SerializeField] NumberFormat numberFormat;
	[SerializeField] int decimalPlaces = 1;
	[SerializeField] string prefix = "";
	[SerializeField] string suffix = "";
	[SerializeField] string separator = " / ";
	[SerializeField] TextMeshProUGUI resourceText;

	CharacterResource resource;
	readonly StringBuilder stringBuilder = new();

	void Start()
	{
		resource = resourceType switch
		{
			ResourceType.Health => Player.Instance.health,
			ResourceType.Energy => Player.Instance.energy,
			ResourceType.Ammo => Player.Instance.ammo,
			_ => null
		};
		resource.OnChanged += Refresh;
		Refresh();
	}

	void Refresh()
	{
		stringBuilder.Clear();
		stringBuilder.Append(prefix);
		stringBuilder.Append(numberFormat == NumberFormat.Integer ? Mathf.RoundToInt(resource.current) : resource.current.ToString($"F{decimalPlaces}"));
		stringBuilder.Append(separator);
		stringBuilder.Append(numberFormat == NumberFormat.Integer ? Mathf.RoundToInt(resource.max) : resource.max.ToString($"F{decimalPlaces}"));
		stringBuilder.Append(suffix);
		resourceText.text = stringBuilder.ToString();
	}
}