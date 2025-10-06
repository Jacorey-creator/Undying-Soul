using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DungeonCreator : MonoBehaviour
{

    [SerializeField] public NavMeshSurface navMeshSurface;
    [SerializeField] public int dungeonWidth, dungeonLength;
    [SerializeField] public int roomWidthMin, roomLengthMin;
    [SerializeField] public int maxIterations;
    [SerializeField] public int corridorWidth;
    [SerializeField] public Material material;

    [Range(0.0f, 0.3f)]
    [SerializeField] public float roomBottomCornerModifier;

    [Range(0.7f, 1.0f)]
    [SerializeField] public float roomTopCornerModifier;

    [Range(0, 2)]
    [SerializeField] public int roomOffset;

    [SerializeField] public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizotalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;

    [Header("Dungeon Transform")]
    [SerializeField] private Vector3 dungeonStartPosition = Vector3.zero;
    [SerializeField] private Vector3 dungeonRotation = Vector3.zero;
    [SerializeField] private Vector3 dungeonScale = Vector3.one;
    
    [Header("Spawning System")]
    [SerializeField] private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();
    [SerializeField] private List<ObjectSpawner> objectSpawners = new List<ObjectSpawner>();
    [SerializeField] private bool enableSpawning = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        DestroyAllChildren();
        
        // Clear all spawners before generating
        ClearAllSpawners();
        
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateRooms(
            maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth);


        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizotalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
            
            // Spawn enemies and objects in rooms only
            if (enableSpawning && listOfRooms[i] is RoomNode)
            {
                // Spawn enemies from all enemy spawners
                foreach (var enemySpawner in enemySpawners)
                {
                    if (enemySpawner != null)
                    {
                        enemySpawner.SpawnEnemies(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, transform, dungeonStartPosition, dungeonRotation, dungeonScale);
                    }
                }
                
                // Spawn objects from all object spawners
                foreach (var objectSpawner in objectSpawners)
                {
                    if (objectSpawner != null)
                    {
                        objectSpawner.SpawnObjects(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, transform, dungeonStartPosition, dungeonRotation, dungeonScale);
                    }
                }
            }
        }

        CreateWalls(wallParent);

        //after generating, rebake the passed navmesh
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    private void ClearAllSpawners()
    {
        foreach (var enemySpawner in enemySpawners)
        {
            if (enemySpawner != null)
            {
                enemySpawner.ClearEnemies();
            }
        }
        
        foreach (var objectSpawner in objectSpawners)
        {
            if (objectSpawner != null)
            {
                objectSpawner.ClearObjects();
            }
        }
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }

        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
        
        // Apply dungeon transform to walls
        wall.transform.position += dungeonStartPosition;
        wall.transform.rotation = Quaternion.Euler(dungeonRotation);
        wall.transform.localScale = Vector3.Scale(wall.transform.localScale, dungeonScale);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV,
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,1,2,
            2,1,3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        // Apply dungeon transform
        dungeonFloor.transform.position = dungeonStartPosition;
        dungeonFloor.transform.rotation = Quaternion.Euler(dungeonRotation);
        dungeonFloor.transform.localScale = dungeonScale;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = transform;


        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizotalPosition);
        }

        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizotalPosition);
        }
        for (int column = (int)bottomLeftV.z; column < (int)topLeftV.z; column++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, column);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int column = (int)bottomRightV.z; column < (int)topRightV.z; column++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, column);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }

    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
