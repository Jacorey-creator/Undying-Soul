using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorsGenerator
{
    public Node EntranceCorridor { get; private set; } // Track the entrance corridor

    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> structureToCheck = new Queue<RoomNode>(
                allNodesCollection.OrderByDescending(node => node.TreeLayerIndex).ToList()
            );

        // Get all existing rooms for collision detection
        List<Node> existingRooms = new List<Node>();
        foreach (var node in allNodesCollection)
        {
            if (node.ChildrenNodeList.Count == 0) // Leaf nodes are rooms
            {
                existingRooms.Add(node);
            }
        }

        while (structureToCheck.Count > 0)
        {
            var node = structureToCheck.Dequeue();

            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }

            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth, existingRooms);
            corridorList.Add(corridor);

            // Don't add corridors to existing rooms list to allow corridor intersections
            // existingRooms.Add(corridor);
        }

        // Create entrance corridor
        if (existingRooms.Count > 0)
        {
            EntranceCorridor = CreateEntranceCorridor(existingRooms, corridorWidth);
            if (EntranceCorridor != null)
            {
                corridorList.Add(EntranceCorridor);
            }
        }

        return corridorList;
    }

    private Node CreateEntranceCorridor(List<Node> existingRooms, int corridorWidth)
    {
        // Find a room closest to the bottom-left corner (or choose your preferred entry point)
        Node entranceRoom = existingRooms.OrderBy(room =>
            Vector2Int.Distance(room.BottomLeftAreaCorner, Vector2Int.zero)).First();

        // Determine which side of the room to place the entrance
        Vector2Int roomCenter = new Vector2Int(
            (entranceRoom.BottomLeftAreaCorner.x + entranceRoom.TopRightAreaCorner.x) / 2,
            (entranceRoom.BottomLeftAreaCorner.y + entranceRoom.TopRightAreaCorner.y) / 2
        );

        // Choose the side that's closest to the dungeon edge
        float leftDist = entranceRoom.BottomLeftAreaCorner.x;
        float rightDist = Mathf.Abs(entranceRoom.TopRightAreaCorner.x);
        float bottomDist = entranceRoom.BottomLeftAreaCorner.y;
        float topDist = Mathf.Abs(entranceRoom.TopRightAreaCorner.y);

        float minDist = Mathf.Min(leftDist, rightDist, bottomDist, topDist);

        int entranceLength = 20; // Length of entrance corridor extending outward

        // Create a dummy "outside" node for the corridor to connect to
        RoomNode outsideNode = new RoomNode();

        if (minDist == leftDist)
        {
            // Entrance on left side
            int y = roomCenter.y - corridorWidth / 2;
            outsideNode.BottomLeftAreaCorner = new Vector2Int(
                entranceRoom.BottomLeftAreaCorner.x - entranceLength - 10,
                y
            );
            outsideNode.TopRightAreaCorner = new Vector2Int(
                entranceRoom.BottomLeftAreaCorner.x - entranceLength,
                y + corridorWidth
            );
        }
        else if (minDist == rightDist)
        {
            // Entrance on right side
            int y = roomCenter.y - corridorWidth / 2;
            outsideNode.BottomLeftAreaCorner = new Vector2Int(
                entranceRoom.TopRightAreaCorner.x + entranceLength,
                y
            );
            outsideNode.TopRightAreaCorner = new Vector2Int(
                entranceRoom.TopRightAreaCorner.x + entranceLength + 10,
                y + corridorWidth
            );
        }
        else if (minDist == bottomDist)
        {
            // Entrance on bottom side
            int x = roomCenter.x - corridorWidth / 2;
            outsideNode.BottomLeftAreaCorner = new Vector2Int(
                x,
                entranceRoom.BottomLeftAreaCorner.y - entranceLength - 10
            );
            outsideNode.TopRightAreaCorner = new Vector2Int(
                x + corridorWidth,
                entranceRoom.BottomLeftAreaCorner.y - entranceLength
            );
        }
        else // top
        {
            // Entrance on top side
            int x = roomCenter.x - corridorWidth / 2;
            outsideNode.BottomLeftAreaCorner = new Vector2Int(
                x,
                entranceRoom.TopRightAreaCorner.y + entranceLength
            );
            outsideNode.TopRightAreaCorner = new Vector2Int(
                x + corridorWidth,
                entranceRoom.TopRightAreaCorner.y + entranceLength + 10
            );
        }

        // Create corridor between entrance room and outside node
        // Pass empty list to avoid collision checks for entrance corridor
        return new CorridorNode(entranceRoom, outsideNode, corridorWidth, new List<Node>());
    }
}

// No longer need this class since we're using CorridorNode
// public class EntranceCorridorNode : Node
// {
//     public EntranceCorridorNode(Vector2Int bottomLeft, Vector2Int topRight) : base(null)
//     {
//         this.BottomLeftAreaCorner = bottomLeft;
//         this.TopRightAreaCorner = topRight;
//     }
// }