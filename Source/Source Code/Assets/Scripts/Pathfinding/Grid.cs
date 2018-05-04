//Reference: Lague, S. (2014). A* Pathfinding Tutorial (Unity). Retrieved from: https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public bool onlyDisplayPathGizmos;      //enable/disable gizmos
    public LayerMask unwalkableMask;        //unwalkable mask
    public Vector2 gridWorldSize;           //grid size
    public float nodeRadius;                //node radius
    Node[,] grid;                           //grid

    float nodeDiameter;                     //node diameter
    int gridSizeX, gridSizeY;               //grid size on x an y axis

    void Start()
    {
        nodeDiameter = nodeRadius * 2;  //set node diameter
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);   //calculate grid size on X axis
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);   //calculate grid size on Y axis
        CreateGrid();

    }

    //get size of the grid
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public Vector3 worldBottomLeft;
    //create grid
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];      //set grid
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        //for each node check if the node should be on walkable layer or not
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    //get the neighbours of the node
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //if x and y are 0, then grid[0][0] wil be the actual node
                if (x == 0 && y == 0)
                    continue;

                //get neighbour position
                int checkX = node.xAxis + x;
                int checkY = node.yAxis + y;

                //check if the neighbour is in grid limits
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //convert node from Vector3 to grid position
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;

    
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }

    }
}