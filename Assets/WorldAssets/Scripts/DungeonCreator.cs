using UnityEngine;

public class DungeonCreator : MonoBehaviour
{

    [SerializeField] public int dungeonWidth, dungeonLength;
    [SerializeField] public int roomWidthMin, roomLengthMin;
    [SerializeField] public int maxIterations;
    [SerializeField] public int corridorWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
    }
}
