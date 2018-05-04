//Reference: Lague, S. (2014). A* Pathfinding Tutorial (Unity). Retrieved from: https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;           //sets if node is walkable or not
    public Vector3 worldPosition;   //node world position
    public int xAxis;               //node  x coordinate
    public int yAxis;               //node  y coordinate
    public Node parent;             //parent

    public int gCost;               //g cost
    public int hCost;               //h cost
    public int fCost;                //fCost=gCost+hCost   

    int heapIndex;
    //Set Node 
    public Node(bool _walkable, Vector3 _worldPos, int _xAxis,int _yAxis)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        xAxis = _xAxis;
        yAxis = _yAxis;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
