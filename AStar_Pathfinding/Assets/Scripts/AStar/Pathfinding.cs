using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{

    Grid grid;

    public Transform seeker, target;

    List<Node> openSet;
    HashSet<Node> closedSet;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        // Gather start and target nodes
        Node startNode = grid.NodeFromWorldPoint(startPosition);
        Node targetNode = grid.NodeFromWorldPoint(targetPosition);
        openSet = new List<Node>(); // the set of nodes to be evaluated
        closedSet = new HashSet<Node>(); // the set of nodes already evaluated

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                // Find node in openSet with lowest fCost value4
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            // Remove current openSet value, add to closedSet
            openSet.Remove(node);
            closedSet.Add(node);

            // Found path
            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            //Jumping
            foreach (Node n in grid.GetJumps()) // Get all nodes that have jump points
            {
                if (!n.pathfindingCheck)
                {
                    if (!openSet.Contains(n))
                        openSet.Add(n);

                    for (int i = 0; i < n.GetJumpLocationsCount(); i++)
                    {
                        Node jumpNode = new Node(Vector3.zero, 0, 0);

                        int x = n.gridX;
                        int y = n.gridY;
                        jumpNode = grid.getNode(x, y);

                        int newCostToNeighbour = node.gCost + GetDistance(node, jumpNode);

                        // Check if neighbour is in openSet, if not add it to the openSet
                        if (!openSet.Contains(jumpNode))
                            openSet.Add(jumpNode);

                        Debug.Log("fCost: " + jumpNode.fCost);
                        Debug.Log("gCost: " + jumpNode.gCost);
                    }

                    n.pathfindingCheck = true;
                }
            }

            // if neighbour is not traversable or neighbour is in closedSet
            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);

                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Gather gCost and hCost values
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    // set parent node to currentNode
                    neighbour.parent = node;

                    // Check if neighbour is in openSet, if not add it to the openSet
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        Debug.Log("Target Node Position:" + currentNode.worldPosition);

        // Retrace path backwards
        while (currentNode != startNode)
        {
            path.Add(currentNode); // Add node to path
            currentNode = currentNode.parent; // Move onto next node(Parent)
        }
        path.Reverse();

        grid.path = path;
    }

    // Calculate distance from nodeA to nodeB
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

}
