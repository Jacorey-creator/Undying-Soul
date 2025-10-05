using System;
using System.Collections.Generic;
using UnityEngine;

internal class RoomGenerator
{
    private int maxIterations;
    private int roomLengthMin;
    private int roomWidthMin;

    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
        this.maxIterations = maxIterations;
        this.roomLengthMin = roomLengthMin;
        this.roomWidthMin = roomWidthMin;
    }

    internal List<RoomNode> GenerateRoomsInSpaces(List<Node> roomSpaces)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftCorner = StructureHelper.GenerateBottomLeftCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.1f, 1
                );
            Vector2Int newTopRightCorner = StructureHelper.GenerateTopRightCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.9f, 1
                );

            space.BottomLeftAreaCorner = newBottomLeftCorner;
            space.TopRightAreaCorner = newTopRightCorner;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightCorner.x, newBottomLeftCorner.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftCorner.x, newTopRightCorner.y);

            listToReturn.Add((RoomNode)space);
        }

        return listToReturn;
    }
}