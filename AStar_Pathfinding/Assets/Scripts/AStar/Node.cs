using UnityEngine;
using System.Collections;

public class Node
{

    public bool pathfindingCheck { get; set; }

    public bool isEdge { get; set; }
    public bool isGround { get; set; }
    public bool isWalkable { get; set; }

    public Vector3 worldPosition; // Nodes position in world

    public int gridX;
    public int gridY;

    public int gCost { get; set; }
    public int hCost { get; set; }

    public Node parent;

    int jumpCount = 0;

    public Vector2[] jumpLocations;
    public Vector2[] fallLocations;
    public Vector2 walkLocation;

    float stepCount;

    public Node(Vector3 _worldPos, int _gridX, int _gridY)
    {
        float h_step = Grid.h_step;
        float s_step = Grid.s_step;
        stepCount = (1 / h_step) * (1 / s_step) + 4;

        jumpLocations = new Vector2[(int)stepCount];
        fallLocations = new Vector2[2];

        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int GetJumpLocationsCount() { return jumpCount; }
    public int GetFallLocationsCount() { return fallLocations.Length; }

    public Vector2 GetJumpLocations(int value)
    {
        return jumpLocations[value];
    }

    public void SetJumpLocation(Vector2 location)
    {
        if (jumpCount != stepCount)
        {
            jumpLocations[jumpCount] = location;

            jumpCount++;
        }
    }

    public Vector2 GetFallLocations(int value)
    {
        return fallLocations[value];
    }

    public void SetFallLocation(Vector2 location, int value)
    {
        fallLocations[value] = location;
    }

    public Vector2 GetWalkLocation()
    {
        return walkLocation;
    }

    public void SetWalkLocation(Vector2 location)
    {
        walkLocation.Set(location.x, location.y);
    }
}