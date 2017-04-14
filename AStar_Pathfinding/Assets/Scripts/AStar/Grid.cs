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
    public Node playerNode;

    float jumpHeight = 3;
    float jump_speed = 3;

    float lerp_timer = 0;

    public static float h_step = 0.25f;
    public static float s_step = 0.25f;
    float t_step = 0.1f;

    float run_Speed = 5;

    float heightDivision;
    float speedDivision;

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
                grid[x, y].isGround = false;
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

                if (y > 0 && y < gridSizeY - 1)
                {
                    grid[x, y].isWalkable = false;

                    if (grid[x, y - 1].isGround && !grid[x, y].isGround)
                    {
                        grid[x, y].isWalkable = true;
                    }
                }

                if (y > 0 && y < gridSizeY - 1 && x > 0 && x < gridSizeX - 1)
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
                if (x == 0 && y == 0)
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

    // get all neighbouring nodes
    public List<Node> GetJumps()
    {
        List<Node> jumpingNodes = new List<Node>();

        foreach(Node n in grid)
        {
            if(n.GetJumpLocationsCount() > 0)
            {
                jumpingNodes.Add(n);
            }
        }

        return jumpingNodes;
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
                Gizmos.color = (n.isWalkable || n.isEdge) ? Color.white : Color.gray;

                if (n.isGround)
                    Gizmos.color = Color.green;

                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }

                if (n.isEdge)
                    Gizmos.color = Color.yellow;


                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }

            // Walking gizmos
            foreach (Node n in grid)
            {
                Gizmos.color = Color.blue;

                if ((n.isEdge || n.isWalkable) && !n.isGround)
                {
                    if (n.GetWalkLocation() != new Vector2(0, 0))
                    {
                        Gizmos.DrawLine(n.worldPosition, n.GetWalkLocation());
                    }
                }
            }

            // Falling gizmos
            foreach (Node n in grid)
            {
                Gizmos.color = Color.yellow;

                if (n.isEdge && !n.isGround)
                {
                    for (int i = 0; i < n.GetFallLocationsCount(); i++)
                    {
                        if (n.GetFallLocations(i) != new Vector2(0, 0))
                        {
                            Gizmos.DrawLine(n.worldPosition, n.GetFallLocations(i));
                        }
                    }
                }
            }

            // Jumping gizmos
            foreach (Node n in grid)
            {
                Gizmos.color = Color.red;

                if (n.isWalkable || n.isEdge && !n.isGround)
                {
                    for (int i = 0; i < n.GetJumpLocationsCount(); i++)
                    {
                        if (n.GetJumpLocations(i) != new Vector2(0, 0))
                        {
                            Gizmos.DrawLine(n.worldPosition, n.GetJumpLocations(i));
                        }
                    }
                }
            }
        }
    }

    public void Calculations()
    {
        // Calculate all walking possibilities on click
        CalculateWalking();

        // Calculate all falling possibilities on click
        //CalculateFalling();

        // Calculate all jumping possibilities on click
        CalculateJumping();

    }

    private void CalculateWalking()
    {
        // Walking Path
        foreach (Node n in grid)
        {
            // Check if block is walkable
            if ((n.isEdge || n.isWalkable) && !n.isGround)
            {
                // Instianiate Nodes
                Node nodeLeft = new Node(Vector3.zero, 0, 0);
                Node nodeRight = new Node(Vector3.zero, 0, 0);

                // Checking if node is within world grid
                if (n.gridX > 0)
                {
                    nodeLeft = grid[n.gridX - 1, n.gridY]; // left
                }
                else
                    break;

                if (n.gridX < gridSizeX - 1)
                {
                    nodeRight = grid[n.gridX + 1, n.gridY]; // Right
                }
                else
                    break;

                // Checking node for walkable surface
                if (nodeRight != null)
                {
                    if ((nodeRight.isEdge || nodeRight.isWalkable) && !nodeRight.isGround)
                    {
                        n.SetWalkLocation(nodeRight.worldPosition); // Set position;
                    }
                    else
                    {
                        // Set to null
                        n.SetWalkLocation(Vector3.zero);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void CalculateFalling()
    {
        // Falling Path
        foreach (Node n in grid)
        {
            int a = 0;
            int b = 0;

            if (n.isEdge)
            {
                if (checkEdge(n) == "Right") // Array to use if its just right hand side fall
                {
                    a = 1;
                    b = 1;
                }
                else if (checkEdge(n) == "Left") // Array to use if its just left hand side fall
                {
                    a = 2;
                    b = 2;
                }
                else if (checkEdge(n) == "Both") // Array to use if its both hand side fall
                {
                    a = 3;
                    b = 3;
                }
            }

            // For each possiblity
            for (int i = 0; i < 1; i++)
            {
                int targetRow = 0;
                int targetRowRight = 0;
                int targetRowLeft = 0;

                // If node is an edge, continue
                if (n.isEdge && !n.isGround)
                {
                    targetRow = n.gridY;

                    targetRowRight = n.gridY;
                    targetRowLeft = n.gridY;

                    bool rightFound = false;
                    bool leftFound = false;

                    while (targetRow > 0)
                    {
                        if (targetRow > 0)
                        {
                            if (a == 3 && b == 3)
                            {
                                Node nodeLeft = new Node(Vector3.zero, 0, 0);
                                Node nodeRight = new Node(Vector3.zero, 0, 0);

                                //Check node to right and get if valid
                                if (!nodeRight.isWalkable || !nodeRight.isEdge && !rightFound)
                                {
                                    nodeRight = grid[n.gridX + 1, targetRowRight];
                                    targetRowRight--;
                                }

                                // Check node to left and get if valid
                                if (!nodeLeft.isWalkable || !nodeLeft.isEdge && !leftFound)
                                {
                                    nodeLeft = grid[n.gridX - 1, targetRowLeft];

                                    targetRowLeft--;
                                }

                                // Check node and put into array
                                if (!nodeLeft.isGround && (nodeLeft.isWalkable || nodeLeft.isEdge) && !leftFound)
                                {
                                    leftFound = true;
                                    n.SetFallLocation(nodeLeft.worldPosition, 0);
                                }
                                else
                                {
                                    n.SetFallLocation(Vector3.zero, 0);
                                }

                                // Check node and put into array
                                if (!nodeRight.isGround && (nodeRight.isWalkable || nodeRight.isEdge) && !rightFound)
                                {
                                    rightFound = true;
                                    n.SetFallLocation(nodeRight.worldPosition, 1);
                                }
                                else
                                {
                                    n.SetFallLocation(Vector3.zero, 1);
                                }

                                if (rightFound && leftFound)
                                    break;
                            }
                            else if ((a == 1 && b == 1) || (a == 2 && b == 2)) // Check both sides
                            {
                                Node nodeToCheck = new Node(Vector3.zero, 0, 0);

                                // When on iteration get right node then left node
                                if (a == 1 && b == 1)
                                    nodeToCheck = grid[n.gridX + 1, targetRow];
                                else if (a == 2 && b == 2)
                                    nodeToCheck = grid[n.gridX - 1, targetRow];

                                // check nodes and put into array
                                if (!nodeToCheck.isGround && (nodeToCheck.isWalkable || nodeToCheck.isEdge))
                                {
                                    n.SetFallLocation(nodeToCheck.worldPosition, i);

                                    break;
                                }
                                else
                                {
                                    n.SetFallLocation(Vector3.zero, i);
                                    n.SetFallLocation(Vector3.zero, i + 1);
                                }
                            }

                            targetRow--;
                        }
                    }
                }
            }
        }
    }

    private void CalculateJumping()
    {
        // Set Divisions
        heightDivision = h_step;
        speedDivision = s_step;

        foreach (Node j in grid)
        {
            // Check if node is valid
            if ((j.isEdge || j.isWalkable) && !j.isGround)
            {
                // Make sure theres no nodes above the jump
                Node temp1 = new Node(Vector3.zero, 0, 0);

                if (j.gridY + 1 < gridSizeY)
                    temp1 = grid[j.gridX, j.gridY + 1];

                Node temp2 = new Node(Vector3.zero, 0, 0);

                if (j.gridY + 2 < gridSizeY)
                    temp2 = grid[j.gridX, j.gridY + 2];


                // Check nodes above
                if (!temp1.isGround && !temp2.isGround)
                {
                    Vector2 startNavpoint = j.worldPosition;

                    // loop for each height division
                    for (float jump = 0; jump < 1.0f; jump += h_step)
                    {
                        // loop for left and right nodes
                        for (int x = -1; x < 2; x += 2)
                        {
                            // loop for each length divisions
                            for (float run = 0; run < 1.0f; run += s_step)
                            {
                                // Velocities
                                float velocityX = run_Speed * run * x;
                                float velocityY = jump_speed * jump;

                                Vector2 velocityTotal = new Vector2(velocityX, velocityY);

                                // Locations
                                float t = 0f;
                                float x_ = 0f;
                                float y_ = 0f;

                                do
                                {

                                    // Gather x and y from velocity
                                    x_ = startNavpoint.x + velocityX * t;
                                    y_ = startNavpoint.y + velocityY - x_ * t - 4.9f * t * t;

                                    // Calculate distance formula
                                    float distance = Mathf.Sqrt((Mathf.Abs(x_ - startNavpoint.x) * Mathf.Abs(x_ - startNavpoint.x)) + (Mathf.Abs(y_ - startNavpoint.y) * Mathf.Abs(y_ - startNavpoint.y)));
                                    float maxDistance = 5;

                                    bool found = false;
                                    if (distance <= maxDistance) // Make sure jumps is within the divisions using distance formula
                                    {
                                        t += t_step;

                                        foreach (Node n in grid) // Try find node that the jump path will land on
                                        {
                                            // Round x and y
                                            x_ = Mathf.RoundToInt(x_);
                                            y_ = Mathf.RoundToInt(y_);

                                            // Get starting position and landing position
                                            Vector2 landingPos = new Vector2(x_, y_);
                                            Vector2 checkPos = n.worldPosition;

                                            if (checkPos == landingPos) // Check if node is landing node
                                            {
                                                if (n.worldPosition != j.worldPosition) // If landing node is not equal to starting node (Same block)
                                                {
                                                    // Check if landing block isGound
                                                    if(n.isGround)
                                                    {
                                                        found = true;
                                                        break;
                                                    }

                                                    // Instianiate Node to left and right of origial start location
                                                    Node nodeLeft = new Node(Vector3.zero, 0, 0);
                                                    Node nodeRight = new Node(Vector3.zero, 0, 0);

                                                    // Checking if node is within world grid
                                                    if (j.gridX > 0)
                                                    {
                                                        nodeLeft = grid[j.gridX - 1, j.gridY]; // left
                                                    }

                                                    if (j.gridX < gridSizeX - 1)
                                                    {
                                                        nodeRight = grid[j.gridX + 1, j.gridY]; // Right
                                                    }

                                                    if(nodeLeft != null)
                                                    {
                                                        if (n.worldPosition == nodeLeft.worldPosition)
                                                            continue;

                                                        if(nodeRight != null)
                                                        {
                                                            if (n.worldPosition == nodeRight.worldPosition)
                                                                continue;
                                                        }
                                                    }

                                                    if (n.isEdge || n.isWalkable)
                                                    {
                                                        found = true;

                                                        j.SetJumpLocation(new Vector2(x_, y_));

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    if (found)
                                        break;
                                }
                                while ((y_ > this.transform.position.y - gridSizeX / 2 && y_ < gridSizeY) || (x_ > this.transform.position.x - gridSizeX / 2 && x_ < gridSizeX));
                            }
                        }
                    }
                }
            }
        }
    }

    public Node getNode(int x, int y)
    {
        return grid[x, y];
    }

    string checkEdge(Node n)
    {
        string direction = "";

        Node leftNode = new Node(Vector3.zero, 0, 0);
        Node rightNode = new Node(Vector3.zero, 0, 0);

        if (n.gridX < gridSizeX && n.gridX > 0)
        {
            rightNode = grid[n.gridX + 1, n.gridY - 1];
            leftNode = grid[n.gridX - 1, n.gridY - 1];
        }

        if (!rightNode.isGround && !leftNode.isGround && !grid[n.gridX + 1, n.gridY].isGround && !grid[n.gridX - 1, n.gridY].isGround)
            direction = "Both";
        else if (!rightNode.isGround && !grid[n.gridX + 1, n.gridY].isGround)
            direction = "Right";
        else if (!leftNode.isGround && !grid[n.gridX - 1, n.gridY].isGround)
            direction = "Left";


        return direction;
    }
}