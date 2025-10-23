
using UnityEngine;

public static class Managers
{
    private static GameObject _root;

    private static PoolManager _pool;
    private static InputManager _input;
    private static void Init()
    {
        if (_root == null)
        {
            _root = GameObject.Find("@Managers");
            if (_root == null)
            {
                _root = new GameObject("@Managers");
                Object.DontDestroyOnLoad(_root);
            }
        }
    }

    private static void CreateManager<T>(ref T manager, string name) where T : Component
    {
        if (manager == null)
        {
            Init();
            GameObject go = new GameObject(name);
            manager = go.AddComponent<T>();
            Object.DontDestroyOnLoad(go);
            go.transform.SetParent(_root.transform, false);
        }
    }

    public static PoolManager Pool
    {
        get
        {
            CreateManager(ref _pool, "PoolManager");
            return _pool;
        }
    }
    public static InputManager Input
    {
        get
        {
            CreateManager(ref _input, "InputManager");
            return _input;
        }
    }
}
