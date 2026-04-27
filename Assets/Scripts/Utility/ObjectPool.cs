using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
	#region Fields
	readonly Func<T> createFunc;
	readonly Queue<T> pool = new();
	int poolIncrementSize = 0;
	bool disableObject = true;
	#endregion

	#region Methods
	public ObjectPool(Func<T> createFunc, int initialSize = 5, bool disableObject = true)
	{
		this.createFunc = createFunc;
		this.disableObject = disableObject;
		poolIncrementSize = initialSize > 0 ? initialSize : 5;
		Fill(poolIncrementSize);
	}

	public T Get()
	{
		if (pool.Count > 0)
		{
			T obj = pool.Dequeue();
			return obj;
		}
		else
		{
			poolIncrementSize *= 2;
			Fill(poolIncrementSize);
			return pool.Dequeue();
		}
	}

	public void Return(T obj)
	{
		if (disableObject)
		{
			obj.gameObject.SetActive(false);
		}
		pool.Enqueue(obj);
	}

	void Fill(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			T obj = createFunc();
			if (disableObject)
			{
				obj.gameObject.SetActive(false);
			}
			pool.Enqueue(obj);
		}
	}
	#endregion
}