using UnityEngine;

public class Room : MonoBehaviour
{
	[System.Serializable]
	public class MonsterSpawnInfo
	{
		public GameObject[] prefabs;
		public Vector2 minPosRatio;
		public Vector2 maxPosRatio;
		public int minCount;
		public int maxCount;
		public Transform parent;
	}

	[Header("--- Wall Settings ---")]
	public Wall wallPrefab;
	public float width = 10f;
	public float length = 10f;
	public float height = 4f;
	public float leftChunkWidth = 2f;
	public float rightChunkWidth = 2f;
	public int doorCount = 1;
	public float doorWidth = 2f;
	public float doorHeight = 2.5f;
	public float wallThickness = 0.5f;

	[Space()]
	[Header("--- Spawn Settings ---")]
	public MonsterSpawnInfo[] spawnInfos;

	WallDoor frontWall;
	Wall leftWall;
	Wall rightWall;
	Wall backWall;

	[ContextMenu("Reset Room")]
	public void ResetRoom()
	{
		if (frontWall != null) DestroyImmediate(frontWall.gameObject);
		if (leftWall != null) DestroyImmediate(leftWall.gameObject);
		if (rightWall != null) DestroyImmediate(rightWall.gameObject);
		if (backWall != null) DestroyImmediate(backWall.gameObject);

		frontWall = null;
		leftWall = null;
		rightWall = null;
		backWall = null;

		foreach (Transform child in transform)
		{
			DestroyImmediate(child.gameObject);
		}

		SetRoom();
	}

	[ContextMenu("Set Room")]
	public void SetRoom()
	{
		if (frontWall == null)
		{
			frontWall = new GameObject("Front Wall").AddComponent<WallDoor>();
			frontWall.transform.SetParent(transform);
			frontWall.wallPrefab = wallPrefab;
		}

		if (leftWall == null)
		{
			leftWall = Instantiate(wallPrefab, transform);
		}

		if (rightWall == null)
		{
			rightWall = Instantiate(wallPrefab, transform);
		}

		if (backWall == null)
		{
			backWall = Instantiate(wallPrefab, transform);
		}


		float rightPos = width * 0.5f - wallThickness * 0.5f;
		float frontPos = length * 0.5f - wallThickness * 0.5f;

		frontWall.name = "Front Wall";
		frontWall.width = width;
		frontWall.height = height;
		frontWall.thickness = wallThickness;
		frontWall.leftChunkWidth = leftChunkWidth;
		frontWall.rightChunkWidth = rightChunkWidth;
		frontWall.doorCount = doorCount;
		frontWall.doorWidth = doorWidth;
		frontWall.doorHeight = doorHeight;
		frontWall.SetWallDoor();
		frontWall.SetTransform(0f, frontPos, 0f, 0f);

		rightWall.name = "Right Wall";
		rightWall.width = length - wallThickness * 2f;
		rightWall.height = height;
		rightWall.thickness = wallThickness;
		rightWall.SetWall();
		rightWall.SetTransform(rightPos, 0f, 0f, 90f);

		leftWall.name = "Left Wall";
		leftWall.width = length - wallThickness * 2f;
		leftWall.height = height;
		leftWall.thickness = wallThickness;
		leftWall.SetWall();
		leftWall.SetTransform(-rightPos, 0f, 0f, 90f);

		backWall.name = "Back Wall";
		backWall.width = width;
		backWall.height = height;
		backWall.thickness = wallThickness;
		backWall.SetWall();
		backWall.SetTransform(0f, -frontPos, 0f, 0f);
	}

	[ContextMenu("Spawn Monsters")]
	public void SpawnMonsters()
	{
		for (int i = 0; i < spawnInfos.Length; i++)
		{
			MonsterSpawnInfo info = spawnInfos[i];
			int count = Random.Range(info.minCount, info.maxCount + 1);
			float posY = info.parent != null ? info.parent.position.y : transform.position.y;
			Quaternion rotation = info.parent != null ? info.parent.rotation : Quaternion.identity;

			for (int j = 0; j < count; j++)
			{
				float posX = Random.Range(info.minPosRatio.x, info.maxPosRatio.x) * width - width * 0.5f + transform.position.x;
				float posZ = Random.Range(info.minPosRatio.y, info.maxPosRatio.y) * length - length * 0.5f + transform.position.z;

				Monster monster = MonsterManager.Instance.GetMonster();
				monster.transform.SetPositionAndRotation(new Vector3(posX, posY, posZ), rotation);
				monster.transform.SetParent(info.parent);
				monster.ToggleActive(true);
			}
		}
	}
}