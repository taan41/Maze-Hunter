using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Cell : MonoBehaviour
{
	public Room[] rooms;
	public GameObject[] lightObjects;
	public int spawnDepth = 1;
	public int lightDepth = 3;

	// [NonSerialized]
	public List<Cell> neighbors = new();
	[NonSerialized]
	public bool spawned = false;
	[NonSerialized]
	public bool lit;

	void Start()
	{
		lit = true;
		SetLight(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SetSpawn(0);
		}
		else if (other.CompareTag("Light Control"))
		{
			SetLight(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Light Control"))
		{
			SetLight(false);
		}
	}

	public void SetCell()
	{
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i] != null)
			{
				rooms[i].SetRoom();
			}
		}
	}

	public void SetSpawn(int depth)
	{
		if (depth > spawnDepth) return;

		if (!spawned)
		{
			spawned = true;

			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i] != null)
				{
					rooms[i].SpawnMonsters();
				}
			}
		}

		for (int i = 0; i < neighbors.Count; i++)
		{
			neighbors[i].SetSpawn(depth + 1);
		}
	}

	public void SetLight(bool flag)
	{
		if (flag && !lit)
		{
			lit = true;

			for (int i = 0; i < lightObjects.Length; i++)
			{
				if (lightObjects[i] != null)
				{
					lightObjects[i].SetActive(true);
				}
			}
		}
		else if (!flag && lit)
		{
			lit = false;

			for (int i = 0; i < lightObjects.Length; i++)
			{
				if (lightObjects[i] != null)
				{
					lightObjects[i].SetActive(false);
				}
			}
		}
	}
}