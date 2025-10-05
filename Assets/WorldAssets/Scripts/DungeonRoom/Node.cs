using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public abstract class Node
{
    private List<Node> childrenNodeList;

    public List<Node> ChildrenNodeList { get => childrenNodeList; }

    public bool Visited { get; set; }
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }

    public Node Parent { get; set; }

    public int TreeLayerIndex { get; set; }

    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.Parent = parentNode;
        if(parentNode != null)
        {
            parentNode.AddChild(this);
            this.TreeLayerIndex = parentNode.TreeLayerIndex + 1;
        }
        else
        {
            this.TreeLayerIndex = 0;
        }
    }

    public void AddChild(Node childNode)
    {
        childrenNodeList.Add(childNode);
    }

    public void RemoveChild(Node childNode)
    {
        childrenNodeList.Remove(childNode);
    }
}