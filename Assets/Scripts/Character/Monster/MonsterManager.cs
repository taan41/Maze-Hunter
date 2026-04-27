using UnityEngine;

public class MonsterManager : MonoBehaviour
{
	public static MonsterManager Instance { get; private set; }

	public Monster[] monsterPrefabs;
	public int poolSize = 10;

	ObjectPool<Monster>[] monsterPools;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Multiple instances of MonsterManager detected. Destroying duplicate.");
			Destroy(gameObject);
			return;
		}
	}

	void Start()
	{
		monsterPools = new ObjectPool<Monster>[monsterPrefabs.Length];
		for (int i = 0; i < monsterPrefabs.Length; i++)
		{
			int index = i; // Capture the current index for the lambda
			monsterPools[i] = new ObjectPool<Monster>(() => CreateMonster(index), poolSize);
		}
	}

	Monster CreateMonster(int index)
	{
		Monster monster = Instantiate(monsterPrefabs[index], transform);
		monster.gameObject.SetActive(false);
		monster.poolIndex = index;
		return monster;
	}

	public Monster GetMonster(int index = -1)
	{
		if (index < 0 || index >= monsterPools.Length)
		{
			index = Random.Range(0, monsterPools.Length);
		}
		return monsterPools[index].Get();
	}

	public void ReturnMonster(Monster monster)
	{
		if (monster.poolIndex >= 0 && monster.poolIndex < monsterPools.Length)
		{
			monsterPools[monster.poolIndex].Return(monster);
		}
		else
		{
			Debug.LogError("Invalid monster pool index: " + monster.poolIndex);
			Destroy(monster.gameObject);
		}
	}
}