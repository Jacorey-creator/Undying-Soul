using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
    [Header("Object Settings")]
    public GameObject prefab;
    public string objectName = "Spawnable Object";
    
    [Header("Spawn Parameters")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
    
    [Range(1, 10)]
    public int minCount = 1;
    
    [Range(1, 10)]
    public int maxCount = 3;
    
    [Header("Positioning")]
    [Range(0f, 0.5f)]
    public float minDistanceFromWalls = 0.1f;
    
    [Range(0f, 0.5f)]
    public float maxDistanceFromWalls = 0.3f;
    
    [Header("Height")]
    public float spawnHeight = 0f;
    
    [Header("Rotation")]
    public bool randomizeRotation = false;
    
    [Range(0f, 360f)]
    public float rotationRange = 360f;
    
    [Header("Scale")]
    public bool randomizeScale = false;
    
    [Range(0.5f, 2f)]
    public float minScale = 0.8f;
    
    [Range(0.5f, 2f)]
    public float maxScale = 1.2f;
}
