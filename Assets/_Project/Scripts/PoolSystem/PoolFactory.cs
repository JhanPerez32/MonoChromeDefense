using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolFactory", menuName = "ScriptableObjects/PoolSystem/PoolFactory")]
public class PoolFactory : ScriptableObject
{
    private readonly Dictionary<Object, Queue<IPoolable>> _pools = new();

    public void Initialize<T>(T prefab, int amount, Transform parent = null) where T : MonoBehaviour, IPoolable
    {
        if (prefab == null)
        {
            Debug.LogWarning("PoolFactory: Tried to initialize null prefab.");
            return;
        }

        if (_pools.TryGetValue(prefab, out var existingPool))
        {
            int needed = amount - existingPool.Count;

            for (int i = 0; i < needed; i++)
            {
                CreateInstance(prefab, parent, existingPool);
            }

            return;
        }

        var newPool = new Queue<IPoolable>();

        for (int i = 0; i < amount; i++)
        {
            CreateInstance(prefab, parent, newPool);
        }

        _pools[prefab] = newPool;
    }

    public T Get<T>(T prefab) where T : MonoBehaviour, IPoolable
    {
        if (!_pools.TryGetValue(prefab, out var queuePoolable))
        {
            Debug.LogWarning($"PoolFactory: Pool not initialized for {prefab}");
            return null;
        }

        if (queuePoolable.Count == 0)
        {
            Debug.LogWarning($"Pool exhausted for {prefab}");
            return null;
        }

        var poolableObject = queuePoolable.Dequeue();
        poolableObject.OnSpawn();

        return (T)poolableObject;
    }
    
    public void Return(IPoolable poolable)
    {
        if (poolable == null)
        {
            Debug.LogWarning("PoolFactory: Tried to return null object.");
            return;
        }

        var prefab = poolable.OriginalPrefab as MonoBehaviour;

        if (!prefab || !_pools.ContainsKey(prefab))
        {
            Debug.LogWarning("PoolFactory: Invalid pool return.");
            return;
        }

        poolable.OnDespawn();
        _pools[prefab].Enqueue(poolable);
    }

    private void CreateInstance<T>(T prefab, Transform parent, Queue<IPoolable> pool) where T : MonoBehaviour, IPoolable
    {
        var instance = Instantiate(prefab, parent);
        var poolable = instance.GetComponent<IPoolable>();

        poolable.OriginalPrefab = prefab;
        poolable.OnDespawn();

        pool.Enqueue(poolable);
    }
}
