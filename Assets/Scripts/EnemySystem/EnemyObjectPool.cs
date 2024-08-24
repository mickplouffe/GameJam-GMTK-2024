using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyObjectPool : MonoBehaviourSingleton<EnemyObjectPool>
{
    [System.Serializable]
    public struct EnemyPool
    {
        public GameObject prefab;
        public int initialPoolSize;
    }

    public EnemyPool[] enemyPools;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var pool in enemyPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialPoolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, HexGridManager.Instance.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    public GameObject GetEnemyObject(GameObject enemyPrefab)
    {
        if (poolDictionary.ContainsKey(enemyPrefab))
        {
            Queue<GameObject> objectPool = poolDictionary[enemyPrefab];

            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(enemyPrefab, HexGridManager.Instance.transform);
                return obj;
            }
        }

        Debug.LogWarning("Enemy prefab not found in the pool dictionary.");
        return null;
    }

    public void ReturnEnemyObject(GameObject enemyPrefab, GameObject obj)
    {
        
        Assert.IsTrue(obj != null, "Returned object should always be true");
        
        if (poolDictionary.ContainsKey(enemyPrefab))
        {
            obj.SetActive(false);
            // obj.transform.position = Vector3.up * 100.0f;
            poolDictionary[enemyPrefab].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Trying to return an enemy to a pool that doesn't exist.");
            Destroy(obj); // Fallback: Destroy the object if it doesn't belong to any pool
        }
    }
}
