using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = Unity.Mathematics.Random;

public class Maze : MonoBehaviour
{
	public enum TwoPathGenType { Both, TwoPath90, TwoPath180 }

	[Serializable]
	public class DepthPathChance
	{
		public int rangeStart;
		public int rangeEnd;
		public int[] specificDepths;
		public float[] pathChances = { 0f, 0f, 0f };
	}

	[Serializable]
	public class CellPrefabInfo
	{
		public Cell cell;
		public int priority;
		public int maxSpawnCount;
	}

	public class CellPrefabTracker
	{
		public Dictionary<int, List<CellPrefabInfo>> priorities = new();
		public Dictionary<CellPrefabInfo, int> counts = new();

		public void Clear()
		{
			priorities.Clear();
			counts.Clear();
		}
	}

	public class CellInfo
	{
		public Cell cell;
		public bool visited;
		public bool[] paths = new bool[4];

		public int PathCount => CountPaths();

		int CountPaths()
		{
			int count = 0;
			for (int i = 0; i < 4; i++)
			{
				if (paths[i]) count++;
			}
			return count;
		}
	}

	const int North = 0, East = 1, South = 2, West = 3;

	[Header("Grid Settings")]
	public int gridWidth = 7;
	public int gridHeight = 7;
	public float cellWorldSize = 14f;
	public string seed;
	public int endDepthStart = 3;
	public float endChancePerDepth = 0.3f;
	[Tooltip("Chance to create a new path based on the number of existing paths (index 0 = chance with 1 existing path, index 1 = chance with 2 existing paths, etc.)")]
	public float[] defaultPathChances = { 1f, 0.6f, 0.5f };
	public DepthPathChance[] customPathChances;
	[Tooltip("Which directions have paths at 0 rotation, starting with North and going clockwise")]
	public bool[] startCellPaths = new bool[4];
	public Vector2Int startCellPosition = new(0, 0);
	[Range(0f, 360f)]
	public float startCellRotation;
	public int maxPathDepth = 1;
	public bool modifyVisitedCellsPath = false;
	public TwoPathGenType twoPathGenType = TwoPathGenType.Both;

	[Header("Prefabs")]
	public Ground ground;
	public Cell startPrefab;
	public CellPrefabInfo[] twoPath90Prefabs;
	public CellPrefabInfo[] twoPath180Prefabs;
	public CellPrefabInfo[] threePathPrefabs;
	public CellPrefabInfo[] fourPathPrefabs;
	public CellPrefabInfo[] endPrefabs;

	[Header("Debug")]
	public bool useSpawnInterval = false;
	public float spawnInterval = 0.1f;

	Random rng;
	CellInfo[,] grid;
	Dictionary<int, float[]> depthPathChancesDict = new();
	List<Cell> spawnedCells = new();
	List<int> validDirs = new();
	List<(int, int)> endCellPositions = new();
	CellPrefabTracker twoPath90Tracker = new();
	CellPrefabTracker twoPath180Tracker = new();
	CellPrefabTracker threePathTracker = new();
	CellPrefabTracker fourPathTracker = new();
	CellPrefabTracker endTracker = new();

	void Awake()
	{
		Clear();
		Generate();
	}

	[ContextMenu("Generate Maze")]
	public void InspectorGenerate()
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
	
	public void Generate()
	{
		SetupTrackers();

		if (!string.IsNullOrEmpty(seed))
		{
			rng = new Random((uint)seed.GetHashCode());
		}
		else
		{
			rng = new Random((uint)DateTime.Now.Ticks);
		}

		grid = new CellInfo[gridWidth, gridHeight];
		for (int x = 0; x < gridWidth; x++)
		for (int y = 0; y < gridHeight; y++)
			grid[x, y] = new CellInfo();
		
		depthPathChancesDict.Clear();
		for (int i = 0; i < customPathChances.Length; i++)
		{
			for (int d = customPathChances[i].rangeStart; d <= customPathChances[i].rangeEnd; d++)
			{
				depthPathChancesDict[d] = customPathChances[i].pathChances;
			}

			for (int s = 0; s < customPathChances[i].specificDepths.Length; s++)
			{
				depthPathChancesDict[customPathChances[i].specificDepths[s]] = customPathChances[i].pathChances;
			}
		}

		GeneratePaths(startCellPosition);

		if (useSpawnInterval)
		{
			StartCoroutine(SpawnCellsWithDelay(startCellPosition));
		}
		else
		{
			SpawnNonEndCells();
			SpawnEndCells();
		}

		float groundWidth = gridWidth * cellWorldSize * 0.1f;
		float groundLength = gridHeight * cellWorldSize * 0.1f;
		float groundX = (gridWidth - 1) * cellWorldSize * 0.5f;
		float groundZ = (gridHeight - 1) * cellWorldSize * 0.5f;
		ground.SetGround(groundX, groundZ, groundWidth, groundLength);
	}

	void SetupTrackers()
	{
		twoPath90Tracker.Clear();
		foreach (CellPrefabInfo info in twoPath90Prefabs)
		{
			if (!twoPath90Tracker.priorities.ContainsKey(info.priority))
			{
				twoPath90Tracker.priorities[info.priority] = new();
			}
			twoPath90Tracker.priorities[info.priority].Add(info);
			twoPath90Tracker.counts[info] = 0;
		}

		twoPath180Tracker.Clear();
		foreach (CellPrefabInfo info in twoPath180Prefabs)
		{
			if (!twoPath180Tracker.priorities.ContainsKey(info.priority))
			{
				twoPath180Tracker.priorities[info.priority] = new();
			}
			twoPath180Tracker.priorities[info.priority].Add(info);
			twoPath180Tracker.counts[info] = 0;
		}

		threePathTracker.Clear();
		foreach (CellPrefabInfo info in threePathPrefabs)
		{
			if (!threePathTracker.priorities.ContainsKey(info.priority))
			{
				threePathTracker.priorities[info.priority] = new();
			}
			threePathTracker.priorities[info.priority].Add(info);
			threePathTracker.counts[info] = 0;
		}

		fourPathTracker.Clear();
		foreach (CellPrefabInfo info in fourPathPrefabs)
		{
			if (!fourPathTracker.priorities.ContainsKey(info.priority))
			{
				fourPathTracker.priorities[info.priority] = new();
			}
			fourPathTracker.priorities[info.priority].Add(info);
			fourPathTracker.counts[info] = 0;
		}

		endTracker.Clear();
		foreach (CellPrefabInfo info in endPrefabs)
		{
			if (!endTracker.priorities.ContainsKey(info.priority))
			{
				endTracker.priorities[info.priority] = new();
			}
			endTracker.priorities[info.priority].Add(info);
			endTracker.counts[info] = 0;
		}
	}

	void GeneratePaths(Vector2Int pos)
	{
		CellInfo cellInfo = grid[pos.x, pos.y];
		cellInfo.visited = true;

		int depth = GetDepth(pos);

		Vector2Int neighborPos;
		CellInfo neighborInfo;

		validDirs.Clear();

		if (depth == 0)
		{
			cellInfo.paths = startCellPaths;

			for (int i = 0; i < 4; i++)
			{
				int dir = startCellRotation == 0 ? i : RotateDirection(i, startCellRotation);

				if (!InBounds(pos + DirectionToVector(dir)))
				{
					cellInfo.paths[dir] = false;
					continue;
				}

				if (cellInfo.paths[dir])
				{
					neighborPos = pos + DirectionToVector(dir);
					neighborInfo = grid[neighborPos.x, neighborPos.y];

					if (!neighborInfo.visited || (modifyVisitedCellsPath && neighborPos != startCellPosition))
					{
						neighborInfo.paths[(dir + 2) % 4] = true;

						if (!neighborInfo.visited)
						{
							GeneratePaths(neighborPos);
						}
					}
				}
			}

			return;
		}
		else if (depth <= maxPathDepth)
		{
			for (int i = 0; i < 4; i++)
			{
				if (!InBounds(pos + DirectionToVector(i))) continue;

				neighborPos = pos + DirectionToVector(i);
				neighborInfo = grid[neighborPos.x, neighborPos.y];

				if (!neighborInfo.visited || (modifyVisitedCellsPath && neighborPos != startCellPosition))
				{
					cellInfo.paths[i] = true;
					neighborInfo.paths[(i + 2) % 4] = true;
				}
			}

			for (int i = 0; i < 4; i++)
			{
				if (cellInfo.paths[i])
				{
					neighborPos = pos + DirectionToVector(i);
					neighborInfo = grid[neighborPos.x, neighborPos.y];

					if (!neighborInfo.visited)
					{
						GeneratePaths(neighborPos);
					}
				}
			}

			if (cellInfo.PathCount <= 1)
			{
				endCellPositions.Add((pos.x, pos.y));
			}

			return;
		}
		else for (int i = 0; i < 4; i++)
		{
			if (!InBounds(pos + DirectionToVector(i))) continue;

			if (!cellInfo.paths[i])
			{
				neighborPos = pos + DirectionToVector(i);
				neighborInfo = grid[neighborPos.x, neighborPos.y];
				
				if (!neighborInfo.visited || (modifyVisitedCellsPath && neighborPos != startCellPosition))
				{
					validDirs.Add(i);
				}
				else if (neighborInfo.paths[(i + 2) % 4])
				{
					cellInfo.paths[i] = true;
				}
			}
		}

		if (depth >= endDepthStart)
		{
			if (rng.NextFloat() < endChancePerDepth * (depth - endDepthStart + 1))
			{
				if (cellInfo.PathCount <= 1)
				{
					endCellPositions.Add((pos.x, pos.y));
				}
				return;
			}
		}

		if (!depthPathChancesDict.TryGetValue(depth, out float[] pathChances))
		{
			pathChances = defaultPathChances;
		}

		while (validDirs.Count > 0)
		{
			float chance = pathChances[Mathf.Clamp(cellInfo.PathCount - 1, 0, pathChances.Length - 1)];
			
			if (rng.NextFloat() > chance) break;

			int dir;
			if (cellInfo.PathCount == 1 && twoPathGenType != TwoPathGenType.Both)
			{
				int existingDir = Array.IndexOf(cellInfo.paths, true);
				int rotatedDir = RotateDirection(existingDir, twoPathGenType == TwoPathGenType.TwoPath90 ? 90 : 180);

				if (validDirs.Contains(rotatedDir))
				{
					dir = rotatedDir;
					validDirs.Remove(rotatedDir);
				}
				else break;
			}
			else
			{
				dir = validDirs.Count > 1 ? validDirs[rng.NextInt(0, validDirs.Count)] : validDirs[0];
				validDirs.Remove(dir);
			}

			cellInfo.paths[dir] = true;

			neighborPos = pos + DirectionToVector(dir);
			neighborInfo = grid[neighborPos.x, neighborPos.y];
			neighborInfo.paths[(dir + 2) % 4] = true;
		}

		if (cellInfo.PathCount <= 1)
		{
			endCellPositions.Add((pos.x, pos.y));
			return;
		}

		for (int i = 0; i < 4; i++)
		{
			if (cellInfo.paths[i])
			{
				neighborPos = pos + DirectionToVector(i);
				neighborInfo = grid[neighborPos.x, neighborPos.y];

				if (!neighborInfo.visited)
				{
					GeneratePaths(neighborPos);
				}
			}
		}
	}

	void SpawnNonEndCells()
	{
		CellInfo startCellInfo = grid[startCellPosition.x, startCellPosition.y];
		startCellInfo.cell = SpawnCell(startCellPosition.x, startCellPosition.y, startPrefab);
		startCellInfo.cell.transform.Rotate(0, startCellRotation, 0);

		for (int x = 0; x < gridWidth; x++)
		for (int y = 0; y < gridHeight; y++)
		{
			CellInfo cellInfo = grid[x, y];
			if (cellInfo.visited && cellInfo.PathCount > 1 && cellInfo.cell == null)
			{
				cellInfo.cell = SpawnCell(x, y);
			}
		}
	}

	void SpawnEndCells()
	{
		while (endCellPositions.Count > 0)
		{
			int posIndex = endCellPositions.Count > 1 ? rng.NextInt(0, endCellPositions.Count) : 0;
			(int x, int y) = endCellPositions[posIndex];
			endCellPositions.RemoveAt(posIndex);

			Cell endCell = SpawnCell(x, y);
			grid[x, y].cell = endCell;
		}
	}

	IEnumerator SpawnCellsWithDelay(Vector2Int pos)
	{
		int x = pos.x;
		int y = pos.y;
		
		CellInfo startCellInfo = grid[x, y];
		startCellInfo.cell = SpawnCell(x, y, startPrefab);
		startCellInfo.cell.transform.Rotate(0, startCellRotation, 0);

		yield return new WaitForSeconds(spawnInterval);

		IEnumerator NonEnd(Vector2Int pos)
		{
			CellInfo cellInfo = grid[pos.x, pos.y];

			if (!cellInfo.visited || cellInfo.PathCount <= 1 || cellInfo.cell != null)
			{
				yield break;
			}

			cellInfo.cell = SpawnCell(pos.x, pos.y);

			yield return new WaitForSeconds(spawnInterval);

			for (int i = 0; i < 4; i++)
			{
				if (cellInfo.paths[i])
				{
					Vector2Int neighborPos = pos + DirectionToVector(i);
					yield return NonEnd(neighborPos);
				}
			}
		}

		IEnumerator End()
		{
			while (endCellPositions.Count > 0)
			{
				int posIndex = endCellPositions.Count > 1 ? rng.NextInt(0, endCellPositions.Count) : 0;
				(int x, int y) = endCellPositions[posIndex];
				endCellPositions.RemoveAt(posIndex);

				CellInfo cellInfo = grid[x, y];
				if (cellInfo.cell == null)
				{
					cellInfo.cell = SpawnCell(x, y);
					yield return new WaitForSeconds(spawnInterval);
				}
			}
		}

		for (int i = 0; i < 4; i++)
		{
			int dir = startCellRotation == 0 ? i : RotateDirection(i, startCellRotation);

			if (startCellInfo.paths[dir])
			{
				Vector2Int neighborPos = new Vector2Int(x, y) + DirectionToVector(dir);
				yield return NonEnd(neighborPos);
			}
		}

		yield return End();
	}

	Cell SpawnCell(int x, int y, Cell prefab = null)
	{
		CellInfo cellInfo = grid[x, y];
		Vector3 position = new(x * cellWorldSize, 0, y * cellWorldSize);

		if (prefab == null)
		{
			prefab = cellInfo.PathCount switch
			{
				1 => GetPrefab(endTracker),
				2 => cellInfo.paths[0] == cellInfo.paths[2] || cellInfo.paths[1] == cellInfo.paths[3] ? GetPrefab(twoPath180Tracker) : GetPrefab(twoPath90Tracker),
				3 => GetPrefab(threePathTracker),
				4 => GetPrefab(fourPathTracker),
				_ => null
			};
		}

		if (prefab != null)
		{
			Cell cell = Instantiate(prefab, position, Quaternion.identity, transform);
			RotateCell(cell.gameObject, cellInfo, cellInfo.PathCount);
			cell.SetCell();
			
			for (int i = 0; i < 4; i++)
			{
				if (cellInfo.paths[i])
				{
					Vector2Int neighborPos = new Vector2Int(x, y) + DirectionToVector(i);
					CellInfo neighbor = grid[neighborPos.x, neighborPos.y];
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

	Cell GetPrefab(CellPrefabTracker tracker)
	{
		if (tracker.priorities.Count == 0) return null;

		foreach (int priority in tracker.priorities.Keys)
		{
			List<CellPrefabInfo> infoList = tracker.priorities[priority];

			while (infoList.Count > 0)
			{
				int infoIndex = infoList.Count > 1 ? rng.NextInt(0, infoList.Count) : 0;
				CellPrefabInfo info = infoList[infoIndex];

				if (info.maxSpawnCount <= 0 || tracker.counts[info] < info.maxSpawnCount)
				{
					tracker.counts[info]++;
					return info.cell;
				}
				else
				{
					infoList.RemoveAt(infoIndex);
				}
			}
		}

		return null;
	}

	void RotateCell(GameObject obj, CellInfo cellPos, int pathCount)
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

	int GetDepth(Vector2Int pos) => Mathf.Abs(pos.x - startCellPosition.x) + Mathf.Abs(pos.y - startCellPosition.y);

	Vector2Int DirectionToVector(int dir) => dir switch
	{
		North => new Vector2Int(0, 1),
		East  => new Vector2Int(1, 0),
		South => new Vector2Int(0, -1),
		West  => new Vector2Int(-1, 0),
		_     => Vector2Int.zero
	};

	int RotateDirection(int dir, float rotation) => (dir + Mathf.RoundToInt(rotation / 90)) % 4;
}