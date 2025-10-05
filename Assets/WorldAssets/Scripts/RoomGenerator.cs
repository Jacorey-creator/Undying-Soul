using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
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

    public List<RoomNode> GenerateRoomsInSpaces(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifier, roomOffset
                );
            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomTopCornerModifier, roomOffset
                );

            // Enforce minimum room dimensions to avoid overly skinny rooms
            int minWidth = Mathf.Max(roomWidthMin, 1);
            int minLength = Mathf.Max(roomLengthMin, 1);

            // Clamp top-right so width/length are at least minimums
            int clampedTopRightX = Mathf.Max(newTopRightPoint.x, newBottomLeftPoint.x + minWidth);
            int clampedTopRightY = Mathf.Max(newTopRightPoint.y, newBottomLeftPoint.y + minLength);
            // Also ensure we don't exceed the containing space
            clampedTopRightX = Mathf.Min(clampedTopRightX, space.TopRightAreaCorner.x - roomOffset);
            clampedTopRightY = Mathf.Min(clampedTopRightY, space.TopRightAreaCorner.y - roomOffset);

            newTopRightPoint = new Vector2Int(clampedTopRightX, clampedTopRightY);

            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            listToReturn.Add((RoomNode)space);
        }

        return listToReturn;
    }
}