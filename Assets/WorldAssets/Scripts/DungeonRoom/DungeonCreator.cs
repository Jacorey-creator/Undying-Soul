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

    private Node entranceCorridor; // Track entrance corridor

    [Header("Generation Settings")]
    [SerializeField] private int maxGenerationAttempts = 5;

    void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        int attempts = 0;
        bool success = false;

        while (!success && attempts < maxGenerationAttempts)
        {
            attempts++;
            try
            {
                DestroyAllChildren();
                ClearAllSpawners();

                GenerateDungeonInternal();
                success = true;

                Debug.Log($"Dungeon generated successfully on attempt {attempts}");
            }
            catch (StackOverflowException)
            {
                Debug.LogWarning($"Dungeon generation failed (stack overflow) on attempt {attempts}. Retrying...");
                DestroyAllChildren();
                ClearAllSpawners();
            }
            catch (Exception e)
            {
                Debug.LogError($"Dungeon generation failed on attempt {attempts}: {e.Message}");
                DestroyAllChildren();
                ClearAllSpawners();
            }
        }

        if (!success)
        {
            Debug.LogError($"Failed to generate dungeon after {maxGenerationAttempts} attempts. Try adjusting generation parameters.");
        }
    }

    private void GenerateDungeonInternal()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateRooms(
            maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth);

        // Store reference to entrance corridor
        entranceCorridor = generator.EntranceCorridor;

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizotalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, listOfRooms[i]);

            // Spawn enemies and objects in rooms only
            if (enableSpawning && listOfRooms[i] is RoomNode)
            {
                foreach (var enemySpawner in enemySpawners)
                {
                    if (enemySpawner != null)
                    {
                        enemySpawner.SpawnEnemies(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, transform, dungeonStartPosition, dungeonRotation, dungeonScale);
                    }
                }

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

        wall.transform.position += dungeonStartPosition;
        wall.transform.rotation = Quaternion.Euler(dungeonRotation);
        wall.transform.localScale = Vector3.Scale(wall.transform.localScale, dungeonScale);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner, Node currentNode)
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

        dungeonFloor.transform.position = dungeonStartPosition;
        dungeonFloor.transform.rotation = Quaternion.Euler(dungeonRotation);
        dungeonFloor.transform.localScale = dungeonScale;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = transform;

        // Skip all wall generation for entrance corridor
        if (currentNode == entranceCorridor)
        {
            return;
        }

        // Check if any edge connects to the entrance corridor to prevent walls there
        bool bottomConnectsToEntrance = false;
        bool topConnectsToEntrance = false;
        bool leftConnectsToEntrance = false;
        bool rightConnectsToEntrance = false;

        if (entranceCorridor != null)
        {
            Vector2Int entranceBottomLeft = entranceCorridor.BottomLeftAreaCorner;
            Vector2Int entranceTopRight = entranceCorridor.TopRightAreaCorner;

            // Check if entrance corridor shares an edge with this room
            // Bottom edge
            if (Mathf.Approximately(bottomLeftCorner.y, entranceTopRight.y) ||
                Mathf.Approximately(bottomLeftCorner.y, entranceBottomLeft.y))
            {
                if (bottomLeftCorner.x < entranceTopRight.x && topRightCorner.x > entranceBottomLeft.x)
                {
                    bottomConnectsToEntrance = true;
                }
            }
            // Top edge
            if (Mathf.Approximately(topRightCorner.y, entranceBottomLeft.y) ||
                Mathf.Approximately(topRightCorner.y, entranceTopRight.y))
            {
                if (bottomLeftCorner.x < entranceTopRight.x && topRightCorner.x > entranceBottomLeft.x)
                {
                    topConnectsToEntrance = true;
                }
            }
            // Left edge
            if (Mathf.Approximately(bottomLeftCorner.x, entranceTopRight.x) ||
                Mathf.Approximately(bottomLeftCorner.x, entranceBottomLeft.x))
            {
                if (bottomLeftCorner.y < entranceTopRight.y && topRightCorner.y > entranceBottomLeft.y)
                {
                    leftConnectsToEntrance = true;
                }
            }
            // Right edge
            if (Mathf.Approximately(topRightCorner.x, entranceBottomLeft.x) ||
                Mathf.Approximately(topRightCorner.x, entranceTopRight.x))
            {
                if (bottomLeftCorner.y < entranceTopRight.y && topRightCorner.y > entranceBottomLeft.y)
                {
                    rightConnectsToEntrance = true;
                }
            }
        }

        // Bottom wall
        if (!bottomConnectsToEntrance)
        {
            for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
            {
                var wallPosition = new Vector3(row, 0, bottomLeftV.z);
                AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizotalPosition);
            }
        }

        // Top wall
        if (!topConnectsToEntrance)
        {
            for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
            {
                var wallPosition = new Vector3(row, 0, topRightV.z);
                AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizotalPosition);
            }
        }

        // Left wall
        if (!leftConnectsToEntrance)
        {
            for (int column = (int)bottomLeftV.z; column < (int)topLeftV.z; column++)
            {
                var wallPosition = new Vector3(bottomLeftV.x, 0, column);
                AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
            }
        }

        // Right wall
        if (!rightConnectsToEntrance)
        {
            for (int column = (int)bottomRightV.z; column < (int)topRightV.z; column++)
            {
                var wallPosition = new Vector3(bottomRightV.x, 0, column);
                AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
            }
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