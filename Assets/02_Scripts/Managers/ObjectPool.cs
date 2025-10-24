using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolTypeCheckable
{
    void EnqueueAfterTypeCheck(MonoBehaviour obj);
    int GetCurrentPoolSize();
}
public class ObjectPool<T> : IPoolTypeCheckable where T : MonoBehaviour
{
    private T prefab;
    private Queue<T> poolQueue = new Queue<T>();
    public Transform Root;

    public ObjectPool(T prefab, int initCount, Transform parent = null)
    {
        this.prefab = prefab;
        Type type = prefab.GetType();
        Root = new GameObject($"{type}_pool").transform;
        if (parent != null) Root.SetParent(parent, false);

        for (int i = 0; i < initCount; i++)
        {
            T inst = GameObject.Instantiate(prefab, Root);
            inst.gameObject.SetActive(false);
            poolQueue.Enqueue(inst);
        }
    }

    public T Dequeue()
    {
        if (poolQueue.Count == 0) return GameObject.Instantiate(prefab, Root);
        T inst = poolQueue.Dequeue();
        inst.gameObject.SetActive(true);
        return inst;
    }

    public void Enqueue(T prefab)
    {
        if (prefab == null) return;
        prefab.gameObject.SetActive(false);
        poolQueue.Enqueue(prefab);
    }

    public void EnqueueAfterTypeCheck(MonoBehaviour obj)
    {
        if (obj is T typeObj)
        {
            Enqueue(typeObj);
        }
    }

    public int GetCurrentPoolSize()
    {
        return poolQueue.Count;
    }
}
