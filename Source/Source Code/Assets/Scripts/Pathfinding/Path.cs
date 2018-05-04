using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Vector3[] path;  //simplified path
    public Vector3[] toatlPath; //initial path
    public int pathLength;  //initial path length
    public GameObject Pathfind;
    public Pathfinding pathFindCode;

    public Transform start; //start position
    public Transform end;   //end position
   

    private void Awake()
    {
        pathFindCode = Pathfind.GetComponent<Pathfinding>();
    }

    //get simplified path
    public void RequestPath(Vector3[] newPath, Vector3 start, Vector3 end)
    {
        path = pathFindCode.FindPath(start, end);
        
        
    }

    //get the length of the initial path
    public int GetPathLength(Vector3 start,Vector3 end)
    {
        toatlPath = pathFindCode.FindPath(start, end);
        pathLength = pathFindCode.pathLength;
        return pathLength;

    }
}
