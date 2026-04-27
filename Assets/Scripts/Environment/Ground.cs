using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(NavMeshSurface))]
public class Ground : MonoBehaviour
{
	public float materialScale = 1f;

	Renderer render;
	NavMeshSurface navMeshSurface;
	Material material;

	public void SetGround(float x, float z, float width, float length)
	{
		if (render == null) render = GetComponent<Renderer>();
		if (navMeshSurface == null) navMeshSurface = GetComponent<NavMeshSurface>();
		if (material == null) material = render.sharedMaterial;

		transform.position = new Vector3(x, 0f, z);
		transform.localScale = new Vector3(width, 1f, length);

		material.mainTextureScale = new Vector2(width * materialScale, length * materialScale);

		navMeshSurface.BuildNavMesh();
	}
}