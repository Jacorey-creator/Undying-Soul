using UnityEngine;

[System.Serializable]
public class SpawnableItem
{
    [Header("Prefab")]
    public GameObject prefab;
    
    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
    
    [Range(1, 5)]
    public int minCount = 1;
    
    [Range(1, 5)]
    public int maxCount = 2;
    
    [Header("Positioning")]
    [Range(0f, 0.5f)]
    public float wallBuffer = 0.1f;
    
    public float spawnHeight = 0f;
}
