using UnityEngine;
using System.Collections;

public class Node
{
    public bool isEdge { get; set; }
    public bool isGround { get; set; }
    public bool isWalkable { get; set; }

    public Vector3 worldPosition; // Nodes position in world

    public int gridX;
    public int gridY;

    public int distance { get; set; }

    public Node parent;

    public Node(Vector3 _worldPos, int _gridX, int _gridY)
    {
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}