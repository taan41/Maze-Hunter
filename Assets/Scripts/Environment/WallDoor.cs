using System.Collections.Generic;
using UnityEngine;

public class WallDoor : MonoBehaviour
{
	public Wall wallPrefab;
	public float width = 10f;
	public float height = 4f;
	public float thickness = 0.5f;
	public float leftChunkWidth = 2f;
	public float rightChunkWidth = 2f;
	public int doorCount = 1;
	public float doorWidth = 2f;
	public float doorHeight = 2.5f;

	Wall leftChunk;
	Wall rightChunk;
	readonly List<Wall> topChunks = new();
	readonly List<Wall> midChunks = new();

	public void SetTransform(float centerX, float centerZ, float bottomY, float rotationY)
	{
		transform.SetLocalPositionAndRotation(new Vector3(centerX, bottomY, centerZ), Quaternion.Euler(0f, rotationY, 0f));
	}

	[ContextMenu("Set Wall Door")]
	public void SetWallDoor()
	{
		if (leftChunk == null)
		{
			leftChunk = Instantiate(wallPrefab, transform);
		}

		if (rightChunk == null)
		{
			rightChunk = Instantiate(wallPrefab, transform);
		}

		// float sideChunkWidth = (width - doorCount * doorWidth - (doorCount - 1) * doorSpacing) * 0.5f;

		leftChunk.name = "Left Chunk";
		leftChunk.width = leftChunkWidth;
		leftChunk.height = height;
		leftChunk.thickness = thickness;
		leftChunk.SetWall();
		leftChunk.SetTransform(-width * 0.5f + leftChunkWidth * 0.5f, 0f, 0f, 0f);

		rightChunk.name = "Right Chunk";
		rightChunk.width = rightChunkWidth;
		rightChunk.height = height;
		rightChunk.thickness = thickness;
		rightChunk.SetWall();
		rightChunk.SetTransform(width * 0.5f - rightChunkWidth * 0.5f, 0f, 0f, 0f);

		float topChunkHeight = height - doorHeight;
		float doorSpacing = (width - leftChunkWidth - rightChunkWidth - doorCount * doorWidth) / Mathf.Max(doorCount - 1, 1);

		for (int i = 0; i < doorCount; i++)
		{
			Wall topChunk;

			if (i < topChunks.Count)
			{
				if (topChunks[i] != null)
				{
					topChunk = topChunks[i];
				}
				else
				{
					topChunk = Instantiate(wallPrefab, transform);
					topChunks[i] = topChunk;
				}
			}
			else
			{
				topChunk = Instantiate(wallPrefab, transform);
				topChunks.Add(topChunk);
			}

			topChunk.name = $"Top Chunk {i}";

			topChunk.width = doorWidth;
			topChunk.height = topChunkHeight;
			topChunk.thickness = thickness;
			topChunk.wallFooting.SetActive(false);
			topChunk.minimapIcon.enabled = false;
			topChunk.SetWall();
			topChunk.SetTransform(
				-width * 0.5f + leftChunkWidth + doorSpacing * i + doorWidth * (i + 0.5f),
				0f,
				doorHeight,
				0f
			);
		}

		for (int i = 0; i < doorCount - 1; i++)
		{
			Wall midChunk;

			if (i < midChunks.Count)
			{
				if (midChunks[i] != null)
				{
					midChunk = midChunks[i];
				}
				else
				{
					midChunk = Instantiate(wallPrefab, transform);
					midChunks[i] = midChunk;
				}
			}
			else
			{
				midChunk = Instantiate(wallPrefab, transform);
				midChunks.Add(midChunk);
			}

			midChunk.name = $"Mid Chunk {i}";

			midChunk.width = doorSpacing;
			midChunk.height = height;
			midChunk.thickness = thickness;
			midChunk.SetWall();
			midChunk.SetTransform(
				-width * 0.5f + leftChunkWidth + doorSpacing * (i + 0.5f) + doorWidth * (i + 1f),
				0f,
				0f,
				0f
			);
		}

		for (int i = topChunks.Count - 1; i >= doorCount; i--)
		{
			if (topChunks[i] != null)
			{
				DestroyImmediate(topChunks[i].gameObject);
			}
			topChunks.RemoveAt(i);
		}

		for (int i = midChunks.Count - 1; i >= doorCount - 1; i--)
		{
			if (midChunks[i] != null)
			{
				DestroyImmediate(midChunks[i].gameObject);
			}
			midChunks.RemoveAt(i);
		}
	}
}