using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Object Prefabs")]
    public List<GameObject> objectPrefabs = new List<GameObject>();
    
    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
    
    [Range(1, 5)]
    public int minObjects = 1;
    
    [Range(1, 5)]
    public int maxObjects = 2;
    
    [Header("Positioning")]
    [Range(0f, 0.5f)]
    public float wallBuffer = 0.1f;
    
    public float spawnHeight = 0f;
    
    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    public void SpawnObjects(Vector2Int bottomLeft, Vector2Int topRight, Transform parent)
    {
        if (Random.value <= spawnChance && objectPrefabs.Count > 0)
        {
            int objectCount = Random.Range(minObjects, maxObjects + 1);
            
            for (int i = 0; i < objectCount; i++)
            {
                GameObject objectPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Count)];
                Vector3 spawnPos = GetSpawnPosition(bottomLeft, topRight);
                GameObject spawned = Instantiate(objectPrefab, spawnPos, Quaternion.identity, parent);
                spawnedObjects.Add(spawned);
            }
        }
    }
    
    private Vector3 GetSpawnPosition(Vector2Int bottomLeft, Vector2Int topRight)
    {
        float roomWidth = topRight.x - bottomLeft.x;
        float roomHeight = topRight.y - bottomLeft.y;
        
        float buffer = roomWidth * wallBuffer;
        
        float x = Random.Range(bottomLeft.x + buffer, topRight.x - buffer);
        float z = Random.Range(bottomLeft.y + buffer, topRight.y - buffer);
        
        return new Vector3(x, spawnHeight, z);
    }
    
    public void ClearObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        spawnedObjects.Clear();
    }
}
