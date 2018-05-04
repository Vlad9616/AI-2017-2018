using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdNPC : MonoBehaviour
{
    public SteeringBehaviours behaviors;
    public Pathfinding pathfinding;

    public GameObject leader;
    public Path pathCode;
    public Vector3[] localWaypoints;
    public GameObject targetPos;

    private void Awake()
    {
        behaviors = GetComponent<SteeringBehaviours>();
    }

}
