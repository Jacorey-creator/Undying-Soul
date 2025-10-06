using System;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator
{
    RoomNode rootNode;
    List<RoomNode> allNodesCollection = new List<RoomNode>();
    private int dungeonWidth, dungeonLength;

    public Node EntranceCorridor { get; private set; }

    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaves(bsp.RootNode);
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);
        CorridorsGenerator corridorGenerator = new CorridorsGenerator();
        var corridorsList = corridorGenerator.CreateCorridor(allNodesCollection, corridorWidth);

        // Store reference to entrance corridor
        EntranceCorridor = corridorGenerator.EntranceCorridor;

        return new List<Node>(roomList).Concat(corridorsList).ToList();
    }
}