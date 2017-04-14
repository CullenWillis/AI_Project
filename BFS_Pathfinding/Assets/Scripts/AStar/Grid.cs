using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public static bool forceUpdate = false;

    public bool gizmos = false;

    public Transform seeker;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    public Node[,] grid;
    float nodeDiameter;
    int gridSizeX = 0;
    int gridSizeY = 0;

    public List<Node> path;

    public int maxJumpHeight = 3;

    public Node playerNode;

    void Start()
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridSizeY / 2;
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (seeker.position.x * nodeDiameter + nodeRadius) + Vector3.up * (seeker.position.y * nodeDiameter + nodeRadius);

        playerNode = new Node(worldPoint, Mathf.RoundToInt(seeker.position.x), Mathf.RoundToInt(seeker.position.y));

        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void Update()
    {
        UpdtateGrid(); // Place block
        Calculations(); // Update location
    }

    public static void ForceUpdate()
    {
        forceUpdate = true;
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridSizeY / 2;

        // Create Normal Collision Map
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);

                grid[x, y] = new Node(worldPoint, x, y);

                grid[x, y].isEdge = false;
                grid[x, y].isWalkable = false;
                grid[x, y].isGround = false;
            }
        }
    }

    void UpdtateGrid()
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridSizeY / 2;

        // Create Normal Collision Map
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);

                // Check Block at position to see if its walkable, if so set true;
                bool isGround = (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                grid[x, y].isGround = isGround;

                // Calculate Walkables
                if (y > 0 && y < gridSizeY - 1)
                {
                    grid[x, y].isWalkable = false;

                    if (grid[x, y - 1].isGround && !grid[x, y].isGround)
                    {
                        grid[x, y].isWalkable = true;
                    }
                }

                // Calculate Edges
                if (y > 1 && y < gridSizeY - 1 && x > 0 && x < gridSizeX - 1)
                {
                    grid[x, y].isEdge = false;

                    // Edge
                    if ((!grid[x - 1, y - 1].isGround || !grid[x + 1, y - 1].isGround) && grid[x, y - 1].isGround && !grid[x, y].isGround)
                    {
                        grid[x, y].isEdge = true;
                        grid[x, y].isWalkable = true;
                    }
                }
            }
        }
    }

    // Convert World Position to Node Position
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {

        //world position.whatyever + gridworldsize.x*0.5 + [0.....gridsizeX] = x
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    // get all neighbouring nodes
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //|| (x != 0 && y != 0)
                if ((x == 0 && y == 0) || (x != 0 && y != 0))
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public void Calculations()
    {
        CalculateJumps();
    }

    public void CalculateJumps()
    {
        bool foundNodeBelow = false;

        foreach (Node n in grid)
        {
            foundNodeBelow = false;

            if (n != null)
            {

                if (n.isEdge)
                {
                    int x = n.gridX;
                    Node checkNode = new Node(Vector3.zero, 0, 0);

                    // Calculate Direction
                    if (!grid[n.gridX + 1, n.gridY - 1].isGround)
                        x++;
                    else
                        x--;


                    int count = -1;
                    for (int y = n.gridY; y > 0; y--)
                    {
                        if (foundNodeBelow)
                            break;

                        if (count >= maxJumpHeight)
                            break;

                        checkNode = grid[x, y - 1];

                        if (checkNode.isGround)
                        {
                            foundNodeBelow = true;
                        }
                        count++;
                    }

                    count = 0;
                    if(foundNodeBelow)
                    {
                        for (int y = n.gridY; y > 0; y--)
                        {
                            checkNode = grid[x, y];

                            if ((!checkNode.isEdge || !checkNode.isWalkable) && !checkNode.isGround)
                            {
                                checkNode.isWalkable = true;
                            }
                            else
                            {
                                break;
                            }

                            count++;
                        }
                    }
                }
            }
        }
    }

    // get all neighbouring nodes
    public List<Node> GetAllNodes()
    {
        List<Node> nodes = new List<Node>();

        foreach (Node n in grid)
        {
            nodes.Add(n);
        }

        return nodes;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            // Grid gizmos

            foreach (Node n in grid)
            {
                if (gizmos)
                {
                    Gizmos.color = (n.isWalkable) ? Color.white : Color.gray;

                    if (n.isGround)
                        Gizmos.color = Color.green;

                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }

                if (path != null)
                {
                    if(path.Contains(n))
                    {

                        Gizmos.color = Color.black;
                        Gizmos.DrawSphere(n.worldPosition, 0.1f);
                    }
                }
            }
        }
    }

}