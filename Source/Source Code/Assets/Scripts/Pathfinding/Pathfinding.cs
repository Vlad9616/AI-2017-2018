//Reference: Lague, S. (2014). A* Pathfinding Tutorial (Unity). Retrieved from: https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour
{
    
    Grid grid;                  //reference to grid
    public int pathLength=0;    //stores path length
    public Vector3[] currentWaypoints;  //stores path
    

    void Awake()
    {
        grid = GetComponent<Grid>();    //get grid
    }

    //find path between a start and end position
    public Vector3[] FindPath(Vector3 startPos, Vector3 targetPos)
    {

        Node startNode = grid.NodeFromWorldPoint(startPos);     //convert Vector3 start to it's position on the grid
        Node targetNode = grid.NodeFromWorldPoint(targetPos);   //convert Vector3 end to it's position on the grid

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);      //create open set
        HashSet<Node> closedSet = new HashSet<Node>();          //create closed set
        openSet.Add(startNode);                                 //add the start node to the open set

        
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();   
            closedSet.Add(currentNode);

            //check if the path reached it's destination
            if (currentNode == targetNode)
            {
                
                currentWaypoints= RetracePath(startNode, targetNode);
                
                return currentWaypoints;
            }

            //get the next waypoint by using neighbours
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                //check if neighbour is walkable or is part of the closed Set
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                //calculate the new cost
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                //check cost for all neighbours and get the one that has the lowest gCost
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                    {
                        //openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return currentWaypoints;
    }

    //Retrace the path from start node to end node
    public Vector3[] RetracePath(Node startNode, Node endNode)
    {
        pathLength = 0;
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            pathLength++;
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }


        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        
        
        return waypoints;
    }

    //calculate the aproximate distance between two nodes
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.xAxis - nodeB.xAxis);
        int dstY = Mathf.Abs(nodeA.yAxis - nodeB.yAxis);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    //This function is used to simplify path as sometimes the AI might move in straight line without chnging it's direction
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].xAxis - path[i].xAxis, path[i - 1].yAxis - path[i].yAxis);
            //check if direction changed
            if (directionNew != directionOld)
            {
                //add waypoint if direction changed
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
}