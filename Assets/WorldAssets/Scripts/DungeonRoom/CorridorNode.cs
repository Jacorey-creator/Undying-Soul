using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;
    private List<Node> existingRooms = new List<Node>();

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    public CorridorNode(Node node1, Node node2, int corridorWidth, List<Node> existingRooms) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;
        this.existingRooms = existingRooms ?? new List<Node>();
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure2AgainstStructure1();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationRightOrLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessRoomInRelationRightOrLeft(Node structure1, Node structure2)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);

        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        var sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();

        // FIXED: Changed > 0 to == 1
        if (sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        else if (sortedLeftStructure.Count > 1) // FIXED: Added safety check
        {
            int maxX = sortedLeftStructure[0].TopRightAreaCorner.x;
            sortedLeftStructure = sortedLeftStructure.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];
        }
        else
        {
            // Fallback if no children found
            leftStructure = structure1;
        }

        var possibleNeighborsInRightStructureList
            = rightStructureChildren.Where(child => GetValidYForNeighborLeftRight(
                    leftStructure.TopRightAreaCorner,
                    leftStructure.BottomRightAreaCorner,
                    child.TopLeftAreaCorner,
                    child.BottomLeftAreaCorner) != -1).ToList();
        if (possibleNeighborsInRightStructureList.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            rightStructure = possibleNeighborsInRightStructureList[0];
        }

        int y = GetValidYForNeighborLeftRight(
            leftStructure.TopRightAreaCorner,
            leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomLeftAreaCorner
        );

        while (y == -1 && sortedLeftStructure.Count > 1)
        {
            sortedLeftStructure = sortedLeftStructure.Where(
                child => child.TopLeftAreaCorner.y != leftStructure.TopLeftAreaCorner.y).ToList();
            if (sortedLeftStructure.Count > 0) // FIXED: Added safety check
            {
                leftStructure = sortedLeftStructure[0];
                y = GetValidYForNeighborLeftRight(
                    leftStructure.TopRightAreaCorner,
                    leftStructure.BottomRightAreaCorner,
                    rightStructure.TopLeftAreaCorner,
                    rightStructure.BottomLeftAreaCorner
                );
            }
            else
            {
                break; // Exit if no more options
            }
        }

        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + corridorWidth);

        // IMPROVED: Only validate against actual rooms, not other corridors
        ValidateAndAdjustCorridorPosition();
    }

    private int GetValidYForNeighborLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                    leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                    leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                    rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                    rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                    rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                    leftNodeUp - new Vector2Int(0, modifierDistanceFromWall)
                ).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                    leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                    rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        return -1;
    }

    private void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
    {
        Node bottomStructure = null;
        List<Node> structureBottomChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);
        Node topStructure = null;
        List<Node> structureTopChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        var sortedBottomStructure = structureBottomChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottomChildren[0];
        }
        else if (sortedBottomStructure.Count > 1) // FIXED: Added safety check
        {
            int maxY = sortedBottomStructure[0].TopLeftAreaCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(children => Math.Abs(maxY - children.TopLeftAreaCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }
        else
        {
            bottomStructure = structure1; // Fallback
        }

        var possibleNeighborsInTopStructureList
            = structureTopChildren.Where(child => GetValidXForNeighborUpDown(
                    bottomStructure.TopLeftAreaCorner,
                    bottomStructure.TopRightAreaCorner,
                    child.BottomLeftAreaCorner,
                    child.BottomRightAreaCorner)
            != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();
        if (possibleNeighborsInTopStructureList.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = possibleNeighborsInTopStructureList[0];
        }

        int x = GetValidXForNeighborUpDown(
            bottomStructure.TopLeftAreaCorner,
            bottomStructure.TopRightAreaCorner,
            topStructure.BottomLeftAreaCorner,
            topStructure.BottomRightAreaCorner
        );

        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(
                child => child.TopLeftAreaCorner.x != bottomStructure.TopLeftAreaCorner.x).ToList();
            if (sortedBottomStructure.Count > 0) // FIXED: Added safety check
            {
                bottomStructure = sortedBottomStructure[0];
                x = GetValidXForNeighborUpDown(
                    bottomStructure.TopLeftAreaCorner,
                    bottomStructure.TopRightAreaCorner,
                    topStructure.BottomLeftAreaCorner,
                    topStructure.BottomRightAreaCorner
                );
            }
            else
            {
                break; // Exit if no more options
            }
        }

        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + corridorWidth, topStructure.BottomLeftAreaCorner.y);

        ValidateAndAdjustCorridorPosition();
    }

    private int GetValidXForNeighborUpDown(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                    bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                    bottomNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                    topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                    topNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        if (bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                    bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                    topNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }
        // FIXED: Changed bottomNodeRight.x >= topNodeRight.x to >= topNodeLeft.x
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                    topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                    bottomNodeRight - new Vector2Int(modifierDistanceFromWall + this.corridorWidth, 0)
                ).x;
        }

        return -1;
    }

    private RelativePosition CheckPositionStructure2AgainstStructure1()
    {
        Vector2 middlePointStructure1Temp = ((Vector2)structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);

        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if ((angle > 45 && angle < 135))
        {
            return RelativePosition.Up;
        }
        else if ((angle > -135 && angle < -45))
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(middlePointStructure2Temp.y - middlePointStructure1Temp.y,
            middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }

    private void ValidateAndAdjustCorridorPosition()
    {
        // IMPROVED: Only check against actual leaf rooms, not corridors
        foreach (var room in existingRooms)
        {
            // Skip if this room is not a leaf node (could be a parent or corridor)
            if (room is CorridorNode)
                continue;

            if (CollisionHelper.RectangleIntersectsRoom(BottomLeftAreaCorner, TopRightAreaCorner,
                room.BottomLeftAreaCorner, room.TopRightAreaCorner))
            {
                AdjustCorridorToAvoidOverlap(room);
                // Re-validate after adjustment to catch cascading overlaps
                ValidateAndAdjustCorridorPosition();
                return; // Exit after first adjustment and re-validation
            }
        }
    }

    private void AdjustCorridorToAvoidOverlap(Node overlappingRoom)
    {
        Vector2Int roomMin = overlappingRoom.BottomLeftAreaCorner;
        Vector2Int roomMax = overlappingRoom.TopRightAreaCorner;

        // Determine corridor orientation
        bool isHorizontal = (TopRightAreaCorner.y - BottomLeftAreaCorner.y) < (TopRightAreaCorner.x - BottomLeftAreaCorner.x);

        if (isHorizontal)
        {
            // Horizontal corridor - adjust Y position
            int corridorCenter = (BottomLeftAreaCorner.y + TopRightAreaCorner.y) / 2;
            int roomCenter = (roomMin.y + roomMax.y) / 2;

            if (corridorCenter < roomCenter)
            {
                // Corridor is below room center, move it below the room
                int newY = roomMin.y - corridorWidth - 1;
                if (newY >= 0) // Ensure we don't go negative
                {
                    BottomLeftAreaCorner = new Vector2Int(BottomLeftAreaCorner.x, newY);
                    TopRightAreaCorner = new Vector2Int(TopRightAreaCorner.x, newY + corridorWidth);
                }
            }
            else
            {
                // Corridor is above room center, move it above the room
                int newY = roomMax.y + 1;
                BottomLeftAreaCorner = new Vector2Int(BottomLeftAreaCorner.x, newY);
                TopRightAreaCorner = new Vector2Int(TopRightAreaCorner.x, newY + corridorWidth);
            }
        }
        else
        {
            // Vertical corridor - adjust X position
            int corridorCenter = (BottomLeftAreaCorner.x + TopRightAreaCorner.x) / 2;
            int roomCenter = (roomMin.x + roomMax.x) / 2;

            if (corridorCenter < roomCenter)
            {
                // Corridor is left of room center, move it left of the room
                int newX = roomMin.x - corridorWidth - 1;
                if (newX >= 0) // Ensure we don't go negative
                {
                    BottomLeftAreaCorner = new Vector2Int(newX, BottomLeftAreaCorner.y);
                    TopRightAreaCorner = new Vector2Int(newX + corridorWidth, TopRightAreaCorner.y);
                }
            }
            else
            {
                // Corridor is right of room center, move it right of the room
                int newX = roomMax.x + 1;
                BottomLeftAreaCorner = new Vector2Int(newX, BottomLeftAreaCorner.y);
                TopRightAreaCorner = new Vector2Int(newX + corridorWidth, TopRightAreaCorner.y);
            }
        }
    }
}