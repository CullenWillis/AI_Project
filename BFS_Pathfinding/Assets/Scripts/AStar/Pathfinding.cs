using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    Grid grid;

    public Transform seeker, target;

    List<Node> frontier;
    HashSet<Node> visted;

    System.Diagnostics.Stopwatch timer;

    void Awake ()
    {
        grid = GetComponent<Grid>();
	}
	
	void Update ()
    {
        timer = new System.Diagnostics.Stopwatch();
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        timer.Start();

        // Gather start and target nodes
        Node startNode = grid.NodeFromWorldPoint(startPosition);
        Node targetNode = grid.NodeFromWorldPoint(targetPosition);

        frontier = new List<Node>(); // the set of nodes to be evaluated
        visted = new HashSet<Node>(); // the set of nodes already evaluated

        frontier.Add(startNode);

        while (frontier.Count > 0)
        {
            Node current = frontier[0];

            // Remove current openSet value, add to closedSet
            frontier.Remove(current);
            visted.Add(current);

            // Found path
            if (current == targetNode)
            {
                timer.Stop();
                Debug.Log(timer.Elapsed);

                RetracePath(startNode, targetNode);
                return;
            }

            // if neighbour is not traversable or neighbour is in closedSet
            foreach (Node next in grid.GetNeighbours(current))
            {
                if (!next.isWalkable || visted.Contains(next))
                {
                    continue;
                }

                // Calculate new distance from start node
                int newCostToNeighbour = 1 + current.distance;

                if (!frontier.Contains(next))
                {
                    // Set the distance from start node
                    next.distance = newCostToNeighbour;

                    // set parent node to currentNode
                    if(next.distance > current.distance)
                        next.parent = current;

                    // Check if neighbour is in openSet, if not add it to the openSet
                    if (!frontier.Contains(next))
                        frontier.Add(next);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        
        // Retrace path backwards
        while (currentNode != startNode)
        {
            path.Add(currentNode); // Add node to path
            currentNode = currentNode.parent; // Move onto next node(Parent)
        }
        path.Reverse();

        grid.path = path;
    }
}
