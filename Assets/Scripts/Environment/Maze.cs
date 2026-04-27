using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Maze : MonoBehaviour
{
	[Serializable]
	public class EndCellSpawnInfo
	{
		public Cell cell;
		public int priority;
		public int maxSpawnCount;
	}

	public class CellPosition
	{
		public Cell cell;
		public bool visited;
		public bool[] paths = new bool[4];
	}

	const int North = 0, East = 1, South = 2, West = 3;

	public float cellSize = 20f;
	public int gridWidth = 10;
	public int gridHeight = 10;
	public int depthMin = 3;
	public float endChancePerDepthOverMin = 0.3f;
	public bool secondCellAlwaysIntersection = true;
	public Vector2Int startCellPosition = new(0, 0);
	[Range(0f, 1f)]
	[Tooltip("Chance to create a path to another cell, based on how many paths the current cell already has. Index 0 is for the first path, index 1 for the second, etc.")]
	public float[] pathChances = { 1f, 0.7f, 0.5f };

	public Ground ground;
	public Cell hallway90Prefab;
	public Cell hallway180Prefab;
	public Cell intersect3Prefab;
	public Cell intersect4Prefab;
	public Cell startCellPrefab;
	public EndCellSpawnInfo[] endCellInfos;

	[NonSerialized]
	public Cell startCell;

	CellPosition[,] grid;
	List<(int, int, Cell)> endCellPositions = new();
	Dictionary<int, List<EndCellSpawnInfo>> endCellPriorities = new();
	Dictionary<EndCellSpawnInfo, int> endCellCounts = new();
	List<Cell> spawnedCells = new();

	void Awake()
	{
		Clear();
		Generate();
	}

	[ContextMenu("Clear")]
	public void Clear()
	{
		for (int i = spawnedCells.Count - 1; i >= 0; i--)
		{
			if (spawnedCells[i] != null)
			{
				DestroyImmediate(spawnedCells[i].gameObject);
			}
		}

		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}

		spawnedCells.Clear();
	}

	[ContextMenu("Generate Maze")]
	public void InspectorGenerate()
	{
		Clear();
		Generate();
	}
	
	public void Generate()
	{
		grid = new CellPosition[gridWidth, gridHeight];
		for (int x = 0; x < gridWidth; x++)
		for (int y = 0; y < gridHeight; y++)
			grid[x, y] = new CellPosition();

		int startX = startCellPosition.x;
		int startY = startCellPosition.y;
		CellPosition start = grid[startX, startY];
		
		start.visited = true;
		start.paths[North] = true;

		startCell = SpawnCell(startX, startY, null, startCellPrefab);
		start.cell = startCell;

		GeneratePaths(startCellPosition + DirectionToVector(North), 0, startCell);
		SpawnEndCells();

		float groundWidth = gridWidth * cellSize * 0.1f;
		float groundLength = gridHeight * cellSize * 0.1f;
		float groundX = (gridWidth - 1) * cellSize * 0.5f;
		float groundZ = (gridHeight - 1) * cellSize * 0.5f;
		ground.SetGround(groundX, groundZ, groundWidth, groundLength);
	}

	void GeneratePaths(Vector2Int currentPos, int depth, Cell prevCell)
	{
		CellPosition current = grid[currentPos.x, currentPos.y];
		current.visited = true;

		int pathCount = 0;
		List<int> possiblePaths = new();

		Vector2Int neighborPos;
		CellPosition neighbor;

		for (int i = 0; i < 4; i++)
		{
			if (!InBounds(currentPos + DirectionToVector(i))) continue;

			if (current.paths[i])
			{
				pathCount++;
			}
			else
			{
				neighborPos = currentPos + DirectionToVector(i);
				neighbor = grid[neighborPos.x, neighborPos.y];
				
				if (!neighbor.visited)
				{
					possiblePaths.Add(i);
				}
				else if (neighbor.paths[(i + 2) % 4])
				{
					current.paths[i] = true;
					pathCount++;
				}
			}
		}

		if (pathCount <= 1 && depth > depthMin && Random.value < (depth - depthMin) * endChancePerDepthOverMin)
		{
			endCellPositions.Add((currentPos.x, currentPos.y, prevCell));
			return;
		}

		while (possiblePaths.Count > 0)
		{
			int dir = possiblePaths[Random.Range(0, possiblePaths.Count)];
			possiblePaths.Remove(dir);

			neighborPos = currentPos + DirectionToVector(dir);
			neighbor = grid[neighborPos.x, neighborPos.y];

			if (neighbor.visited) continue;
			if ((depth > 0 || !secondCellAlwaysIntersection) && Random.value > pathChances[Mathf.Clamp(pathCount - 1, 0, 3)]) break;

			current.paths[dir] = true;
			pathCount++;

			neighbor.paths[(dir + 2) % 4] = true;
		}

		if (pathCount <= 1)
		{
			endCellPositions.Add((currentPos.x, currentPos.y, prevCell));
			return;
		}

		Cell spawnedCell = SpawnCell(currentPos.x, currentPos.y, prevCell);
		grid[currentPos.x, currentPos.y].cell = spawnedCell;

		for (int i = 0; i < 4; i++)
		{
			if (current.paths[i])
			{
				neighborPos = currentPos + DirectionToVector(i);
				neighbor = grid[neighborPos.x, neighborPos.y];
				if (!neighbor.visited)
				{
					GeneratePaths(neighborPos, depth + 1, spawnedCell);
				}
			}
		}
	}

	Cell SpawnCell(int x, int y, Cell prevCell, Cell prefab = null)
	{
		CellPosition cellPos = grid[x, y];
		Vector3 position = new(x * cellSize, 0, y * cellSize);
		int pathCount = 0;

		for (int i = 0; i < 4; i++)
			if (cellPos.paths[i]) pathCount++;

		if (prefab == null)
		{
			prefab = pathCount switch
			{
				2 => cellPos.paths[0] == cellPos.paths[2] || cellPos.paths[1] == cellPos.paths[3] ? hallway180Prefab : hallway90Prefab,
				3 => intersect3Prefab,
				4 => intersect4Prefab,
				_ => null
			};
		}

		if (prefab != null)
		{
			Cell cell = Instantiate(prefab, position, Quaternion.identity, transform);
			RotateCell(cell.gameObject, cellPos, pathCount);
			cell.SetCell();
			
			for (int i = 0; i < 4; i++)
			{
				if (cellPos.paths[i])
				{
					Vector2Int neighborPos = new Vector2Int(x, y) + DirectionToVector(i);
					CellPosition neighbor = grid[neighborPos.x, neighborPos.y];
					if (neighbor.cell != null)
					{
						cell.neighbors.Add(neighbor.cell);
						neighbor.cell.neighbors.Add(cell);
					}
				}
			}

			spawnedCells.Add(cell);
			return cell;
		}
		return null;
	}

	void SpawnEndCells()
	{
		endCellPriorities.Clear();

		for (int i = 0; i < endCellInfos.Length; i++)
		{
			if (!endCellPriorities.ContainsKey(endCellInfos[i].priority))
			{
				endCellPriorities[endCellInfos[i].priority] = new();
			}
			endCellPriorities[endCellInfos[i].priority].Add(endCellInfos[i]);
			endCellCounts[endCellInfos[i]] = 0;
		}

		foreach (int priority in endCellPriorities.Keys)
		{
			if (endCellPositions.Count == 0) break;

			List<EndCellSpawnInfo> infoList = endCellPriorities[priority];

			while (infoList.Count > 0 && endCellPositions.Count > 0)
			{
				int infoIndex = Random.Range(0, infoList.Count);

				while (infoList[infoIndex].maxSpawnCount > 0 && endCellCounts[infoList[infoIndex]] >= infoList[infoIndex].maxSpawnCount)
				{
					infoList.RemoveAt(infoIndex);
					if (infoList.Count == 0) break;

					infoIndex = Random.Range(0, infoList.Count);
				}

				if (infoList.Count == 0) break;

				EndCellSpawnInfo info = infoList[infoIndex];

				int posIndex = Random.Range(0, endCellPositions.Count);
				(int x, int y, Cell prevCell) = endCellPositions[posIndex];
				endCellPositions.RemoveAt(posIndex);

				SpawnCell(x, y, prevCell, info.cell);
				endCellCounts[info]++;
			}
		}
	}

	void RotateCell(GameObject obj, CellPosition cellPos, int pathCount)
	{
		if (pathCount == 3)
		{
			if (!cellPos.paths[East]) obj.transform.Rotate(0, 90, 0);
			else if (!cellPos.paths[South]) obj.transform.Rotate(0, 180, 0);
			else if (!cellPos.paths[West]) obj.transform.Rotate(0, 270, 0);
			return;
		}
		if (pathCount == 2)
		{
			if (cellPos.paths[South])
			{
				if (cellPos.paths[West]) obj.transform.Rotate(0, 90, 0);
			}
			else if (cellPos.paths[East])
			{
				if (cellPos.paths[North]) obj.transform.Rotate(0, 270, 0);
				else if (cellPos.paths[West]) obj.transform.Rotate(0, 90, 0);
			}
			else if (cellPos.paths[North])
			{
				if (cellPos.paths[West]) obj.transform.Rotate(0, 180, 0);
			}
			return;
		}
		if (pathCount == 1)
		{
			if (cellPos.paths[East]) obj.transform.Rotate(0, 270, 0);
			else if (cellPos.paths[North]) obj.transform.Rotate(0, 180, 0);
			else if (cellPos.paths[West]) obj.transform.Rotate(0, 90, 0);
		}
	}

	bool InBounds(Vector2Int pos) => pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;

	Vector2Int DirectionToVector(int dir) => dir switch
	{
		North => new Vector2Int(0, 1),
		East  => new Vector2Int(1, 0),
		South => new Vector2Int(0, -1),
		West  => new Vector2Int(-1, 0),
		_     => Vector2Int.zero
	};
}