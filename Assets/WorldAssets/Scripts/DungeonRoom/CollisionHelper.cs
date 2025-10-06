using System.Collections.Generic;
using UnityEngine;

public static class CollisionHelper
{
    public static bool RectanglesOverlap(Vector2Int rect1Min, Vector2Int rect1Max, Vector2Int rect2Min, Vector2Int rect2Max)
    {
        return !(rect1Max.x <= rect2Min.x || rect2Max.x <= rect1Min.x || 
                 rect1Max.y <= rect2Min.y || rect2Max.y <= rect1Min.y);
    }
    
    public static bool RectangleContainsPoint(Vector2Int rectMin, Vector2Int rectMax, Vector2Int point)
    {
        return point.x >= rectMin.x && point.x <= rectMax.x && 
               point.y >= rectMin.y && point.y <= rectMax.y;
    }
    
    public static bool RectangleIntersectsRoom(Vector2Int corridorMin, Vector2Int corridorMax, Vector2Int roomMin, Vector2Int roomMax)
    {
        // Add small buffer to prevent corridors from touching room edges
        Vector2Int bufferedRoomMin = roomMin + Vector2Int.one;
        Vector2Int bufferedRoomMax = roomMax - Vector2Int.one;
        
        return RectanglesOverlap(corridorMin, corridorMax, bufferedRoomMin, bufferedRoomMax);
    }
    
    public static Vector2Int FindValidCorridorPosition(Vector2Int startPos, Vector2Int endPos, int corridorWidth, 
        List<Node> existingRooms, int maxAttempts = 10)
    {
        Vector2Int bestPosition = startPos;
        float bestDistance = Vector2Int.Distance(startPos, endPos);
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2Int testPos = startPos;
            
            // Try different corridor positions
            if (attempt > 0)
            {
                int offsetX = Random.Range(-corridorWidth, corridorWidth + 1);
                int offsetY = Random.Range(-corridorWidth, corridorWidth + 1);
                testPos = startPos + new Vector2Int(offsetX, offsetY);
            }
            
            Vector2Int corridorMin = testPos;
            Vector2Int corridorMax = testPos + new Vector2Int(corridorWidth, corridorWidth);
            
            bool validPosition = true;
            
            // Check collision with existing rooms
            foreach (var room in existingRooms)
            {
                if (RectangleIntersectsRoom(corridorMin, corridorMax, room.BottomLeftAreaCorner, room.TopRightAreaCorner))
                {
                    validPosition = false;
                    break;
                }
            }
            
            if (validPosition)
            {
                float distance = Vector2Int.Distance(testPos, endPos);
                if (distance < bestDistance)
                {
                    bestPosition = testPos;
                    bestDistance = distance;
                }
            }
        }
        
        return bestPosition;
    }
}
