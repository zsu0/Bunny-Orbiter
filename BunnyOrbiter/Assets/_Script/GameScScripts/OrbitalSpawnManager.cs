using UnityEngine;
using System.Collections.Generic;

public class OrbitalSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class Spawnable
    {
        public GameObject prefab;
        [Range(0, 100)] public int spawnChance;
        public int poolSize = 20;
        [HideInInspector] public Queue<GameObject> pool;
    }

    [Header("Spawnables")]
    public Spawnable[] objects = new Spawnable[]
    {
        new Spawnable { spawnChance = 60 }, // Obstacles
        new Spawnable { spawnChance = 20 }, // Carrots
        new Spawnable { spawnChance = 10 }  // Cabbages
    };

    [Header("Spawn Area")]
    public float minRadius = 5f;
    public float maxRadius = 25f;
    public float safePlayerRadius = 1.5f;
    public int initialSpawnCount = 30;

    private Transform moon;
    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        moon = GameObject.FindGameObjectWithTag("Moon").transform;
        InitializePools();
        GenerateInitialObjects();
    }

    void InitializePools()
    {
        foreach (Spawnable spawnable in objects)
        {
            spawnable.pool = new Queue<GameObject>();
            for (int i = 0; i < spawnable.poolSize; i++)
            {
                GameObject obj = Instantiate(spawnable.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                spawnable.pool.Enqueue(obj);
            }
        }
    }

    void GenerateInitialObjects()
    {
        float circumference = 2 * Mathf.PI * ((minRadius + maxRadius) / 2);
        int maxObjects = Mathf.FloorToInt(circumference / safePlayerRadius);

        for (int i = 0; i < Mathf.Min(initialSpawnCount, maxObjects); i++)
        {
            SpawnObject();
        }
    }

    public void SpawnObject()
    {
        Vector3 spawnPos;
        int attempts = 0;
        
        do
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float radius = Random.Range(minRadius, maxRadius);
            spawnPos = moon.position + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            attempts++;
        } 
        while (!IsPositionValid(spawnPos) && attempts < 30);

        if (attempts < 30)
        {
            GameObject obj = GetRandomObject();
            if (obj != null)
            {
                obj.transform.position = spawnPos;
                obj.SetActive(true);
                spawnedPositions.Add(spawnPos);
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 pos in spawnedPositions)
        {
            if (Vector3.Distance(position, pos) < safePlayerRadius)
                return false;
        }
        return true;
    }

    GameObject GetRandomObject()
    {
        int totalChance = 0;
        foreach (Spawnable spawnable in objects)
        {
            totalChance += spawnable.spawnChance;
        }

        int random = Random.Range(0, totalChance);
        int current = 0;

        foreach (Spawnable spawnable in objects)
        {
            current += spawnable.spawnChance;
            if (random < current && spawnable.pool.Count > 0)
            {
                return spawnable.pool.Dequeue();
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        foreach (Spawnable spawnable in objects)
        {
            if (obj.CompareTag(spawnable.prefab.tag))
            {
                spawnable.pool.Enqueue(obj);
                break;
            }
        }
    }
}