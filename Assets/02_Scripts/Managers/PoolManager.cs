using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    private Dictionary<Type, IPoolTypeCheckable> pools = new Dictionary<Type, IPoolTypeCheckable>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePool<T>(T prefab, int initCount, Transform parent = null) where T : MonoBehaviour
    {
        if (prefab == null) return;
        Type type = prefab.GetType();
        if (pools.ContainsKey(type)) return;
        pools.Add(type, new ObjectPool<T>(prefab, initCount, parent));
    }

    public T GetFromPool<T>(T prefab) where T : MonoBehaviour
    {
        Type type = prefab.GetType();
        if (!pools.ContainsKey(type)) return null;
        ObjectPool<T> pool = pools[type] as ObjectPool<T>;
        if (pool == null) return null;
        return pool.Dequeue();
    }

    public void ReturnToPool<T>(T prefab) where T : MonoBehaviour
    {
        if (prefab == null) return;
        Type type = prefab.GetType();
        if (!pools.ContainsKey(type))
        {
            Destroy(prefab.gameObject);
            return;
        }
        ObjectPool<T> pool = pools[type] as ObjectPool<T>;
        if (pool == null)
        {
            Destroy(prefab.gameObject);
            return;
        }
        pool.Enqueue(prefab);
    }
}
