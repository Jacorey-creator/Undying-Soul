using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorsGenerator
{
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
            
            if(node.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            
            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth, existingRooms);
            corridorList.Add(corridor);
            
            // Add this corridor to existing rooms for future collision detection
            existingRooms.Add(corridor);
        }

        return corridorList;
    }
}