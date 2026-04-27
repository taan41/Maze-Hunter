using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Wall : MonoBehaviour
{
	public GameObject wallFooting;
	public SpriteRenderer minimapIcon;
	public float width = 1f;
	public float height = 1f;
	public float thickness = 0.5f;
	public float footingHeight = 0.1f;
	public float footingThickness = 0.2f;

	void OnValidate()
	{
		SetWall();
	}

	public void SetWall()
	{
		transform.localScale = new Vector3(width, height, thickness);
		if (wallFooting != null)
		{
			wallFooting.transform.localScale = new Vector3(
				(width + footingThickness) / width,
				footingHeight / height,
				(thickness + footingThickness) / thickness
			);
			wallFooting.transform.localPosition = new Vector3(0f, -0.5f + footingHeight / height * 0.5f, 0f);
		}

	}

	public void SetTransform(float centerX, float centerZ, float bottomY, float rotationY)
	{
		transform.SetLocalPositionAndRotation(new Vector3(centerX, bottomY + height * 0.5f, centerZ), Quaternion.Euler(0f, rotationY, 0f));
	}
}