using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class UIGradient : BaseMeshEffect
{
	public enum GradientDirection
	{
		Horizontal,
		Vertical
	}
	
	public GradientDirection direction = GradientDirection.Horizontal;
	public Color colorA = Color.white;
	public Color colorB = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        UIVertex vertex = default;

        for (int i = 0; i < vh.currentVertCount; i++)
		{
			vh.PopulateUIVertex(ref vertex, i);

			float t = direction == GradientDirection.Horizontal
				? vertex.position.x / GetComponent<RectTransform>().rect.width
				: vertex.position.y / GetComponent<RectTransform>().rect.height;

			vertex.color = Color.Lerp(colorA, colorB, t);
			vh.SetUIVertex(vertex, i);
		}
    }
}