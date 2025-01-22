using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public Vector2 prefabSizeVariation;
        public Transform parent;
    }

    #region Singleton

    public static Object_Pooler Instance;

    private void Awake()
    {
        Instance = this;
        start();
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.transform.localScale *= Random.Range(pool.prefabSizeVariation.x, pool.prefabSizeVariation.y);
                    obj.SetActive(false);
                    if (pool.parent != null)
                    {
                        obj.transform.SetParent(pool.parent);
                    }
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position , Quaternion rotation = new Quaternion())
    {
        if (!poolDictionary.ContainsKey(tag)) { return null; }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}
