using UnityEngine;

public class GridGizmo : MonoBehaviour
{
	public Color color = Color.black;
	public int gridWidth = 7;
	public int gridHeight = 7;
	public float cellSize = 14f;
	public Vector2Int originCell = new(3, 3);
	public Sprite lineSprite;
	public float lineWidth = 0.3f;
	public bool drawDepthMap = true;
	public float numberSize = 0.8f;
	public Color numberColor = Color.black;
	public Color minDepthColor = Color.white;
	public Color maxDepthColor = Color.white;


	[ContextMenu("Draw Grid")]
	public void DrawGrid()
	{
		ClearGrid();
		
		for (int x = 0; x <= gridWidth; x++)
		{
			GameObject line = new($"Vertical line {x}")
			{
				layer = LayerMask.NameToLayer("Minimap")
			};
			line.transform.parent = transform;
			line.transform.SetLocalPositionAndRotation(new Vector3(x * cellSize - cellSize / 2, 0, (gridHeight - 1) * cellSize / 2), Quaternion.Euler(90, 0, 0));
			line.transform.localScale = new Vector3(lineWidth, gridHeight * cellSize, 1);
			SpriteRenderer sr = line.AddComponent<SpriteRenderer>();
			sr.sprite = lineSprite;
			sr.color = color;
		}

		for (int y = 0; y <= gridHeight; y++)
		{
			GameObject line = new($"Horizontal line {y}")
			{
				layer = LayerMask.NameToLayer("Minimap")
			};
			line.transform.parent = transform;
			line.transform.SetLocalPositionAndRotation(new Vector3((gridWidth - 1) * cellSize / 2, 0, y * cellSize - cellSize / 2), Quaternion.Euler(90, 0, 0));
			line.transform.localScale = new Vector3(gridWidth * cellSize, lineWidth, 1);
			SpriteRenderer sr = line.AddComponent<SpriteRenderer>();
			sr.sprite = lineSprite;
			sr.color = color;
		}

		if (drawDepthMap)
		{
			int maxDepth = Mathf.Max(Mathf.Abs(gridWidth - 1 - originCell.x), originCell.x) + Mathf.Max(Mathf.Abs(gridHeight - 1 - originCell.y), originCell.y);
			
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					int depth = Mathf.Abs(x - originCell.x) + Mathf.Abs(y - originCell.y);

					GameObject depthBackgroundObj = new($"Depth background {x},{y}")
					{
						layer = LayerMask.NameToLayer("Minimap")
					};
					depthBackgroundObj.transform.parent = transform;
					depthBackgroundObj.transform.SetLocalPositionAndRotation(new Vector3(x * cellSize, -0.1f, y * cellSize), Quaternion.Euler(90, 0, 0));
					depthBackgroundObj.transform.localScale = new Vector3(cellSize, cellSize, 1);

					SpriteRenderer sr = depthBackgroundObj.AddComponent<SpriteRenderer>();
					sr.sprite = lineSprite;
					sr.color = Color.Lerp(minDepthColor, maxDepthColor, (float)depth / maxDepth);

					GameObject depthNumberObj = new($"Depth Number {x},{y}")
					{
						layer = LayerMask.NameToLayer("Minimap")
					};
					depthNumberObj.transform.parent = transform;
					depthNumberObj.transform.SetLocalPositionAndRotation(new Vector3(x * cellSize, 0, y * cellSize), Quaternion.Euler(90, 0, 0));

					TextMesh tm = depthNumberObj.AddComponent<TextMesh>();
					tm.text = depth.ToString();
					tm.fontSize = 100;
					tm.characterSize = numberSize;
					tm.anchor = TextAnchor.MiddleCenter;
					tm.color = numberColor;
				}
			}
		}
	}

	[ContextMenu("Clear Grid")]
	public void ClearGrid()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}
}