using UnityEngine;

public class Line
{
    Orientation orientation;
    Vector2Int coordinates;

    public Line(Orientation orientation, Vector2Int coordinate)
    {
        this.orientation = orientation;
        this.coordinates = coordinate;
    }

    public Orientation Orientation { get => orientation; set => this.orientation = value; }
    public Vector2Int Coordinates { get => coordinates; set => this.coordinates = value; }
}

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}