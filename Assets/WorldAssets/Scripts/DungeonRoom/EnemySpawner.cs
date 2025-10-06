using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    
    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float spawnChance = 0.7f;
    
    [Range(1, 5)]
    public int minEnemies = 1;
    
    [Range(1, 5)]
    public int maxEnemies = 3;
    
    [Header("Positioning")]
    [Range(0f, 0.5f)]
    public float wallBuffer = 0.1f;
    
    public float spawnHeight = 0f;
    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    public void SpawnEnemies(Vector2Int bottomLeft, Vector2Int topRight, Transform parent, Vector3 dungeonPosition, Vector3 dungeonRotation, Vector3 dungeonScale)
    {
        if (Random.value <= spawnChance && enemyPrefabs.Count > 0)
        {
            int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
            
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                Vector3 spawnPos = GetSpawnPosition(bottomLeft, topRight);
                
                // Apply dungeon transform
                spawnPos += dungeonPosition;
                
                GameObject spawned = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(dungeonRotation), parent);
                spawned.transform.localScale = Vector3.Scale(spawned.transform.localScale, dungeonScale);
                spawnedEnemies.Add(spawned);
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
    
    public void ClearEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null) DestroyImmediate(enemy);
        }
        spawnedEnemies.Clear();
    }
}
