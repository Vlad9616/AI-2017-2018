using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Vehicle))]
public class SteeringBehaviours : MonoBehaviour
{
    public bool isCrowd=false;
    [Header("Core/Objects Reference")]
    public Vehicle vehicle;        //reference to vehicle class
    public Vector3 VelocitySum = Vector3.zero; //used to calculate the overall force 

    public NPC npc;
    public CrowdNPC crowdNPC;

    [Header("Lists/Arrays")]
    public List<Transform> globalWaypoints; //stores the global predefined waypoints
    public Vector3[] globalVector;
    public Vector3[] localWaypoints;        //stores the local waypoints between global waypoints
    public GameObject[] obstacles;           //stores obstacles that need to be avoided by the AI

    public Vector3[] pathToTarget;          //stores the path to a set target
    public List<GameObject> Neighbour=new List<GameObject>();          //stores another agents that are around
    public GameObject[] agents;             //store agents present in the game world
    public GameObject[] followers;          //stores followers agents

    //Indexes
    public int localIndex=-1;               //local index set to default value of -1
    public int globalIndex=-1;              //global index set to default value of -1

    //ToTargetPahth
    public int targetIndex = -1;            //target index set to default value of -1
    public Vector3 pathOnTarget;            //stores the path to a certain target


    //SB WEIGHTS
    public float seekWeight = 0.05f;   
    public float fleeWeight = 0.00001f;
    public float arriveWeight = 0.00001f;
    public float pursuitWeight = 0.00001f;
    public float evadeWeight = 0.00001f;
    public float hideWeight = 0.00001f;
    public float wallAvoidanceWeight = 0.00001f;
    public float pathFollowingWeight = 0.00001f;
    public float toTargetPathWeight = 0.00001f;
    public float wanderWeight = 0.01f;
    public float separationWeihgt = 0.0003f;
    public float alignmentWeight = 0.000001f;
    public float cohesionWeight = 0.000001f;

    [Header("Conditions")]
    //Condition
    

    public bool IsSeekOn = false;
    public bool IsFleeOn = false;
    public bool isArriveOn = false;
    public bool isPursuitOn = false;
    public bool isOffPursuitOn = false;
    public bool isEvadeOn = false;
    public bool isHideOn = false;
    public bool iswallAvoid = false;
    public bool isPathOn = false;
    public bool isToTargetPathOn = false;
    public bool IsWanderOn = false;
    public bool IsSeparationOn = false;
    public bool IsAlingmentOn = false;
    public bool isCohesionOn = false;

    //SeekOn
    public Vector3 SeekOnTargetPos;
    float SeekOnStopDistance;

    //FleeOn
    Vector3 FleeFromTargetPos;
    float FleeFromStopDistance;

    //Arrive
    Vector3 ArriveOnTargetPos;
    float ArriveOnStopDistance;

    //WanderOn
    public float WanderRadius = 10f;
    public float WanderDistance = 10f;
    public float WanderJitter = 1f;
    Vector3 WanderTarget = Vector3.zero;

    //Hide
    public Vector3 hideFromPos;
    public Vector3 lastHide;
    public Vector3 prevSpot=Vector3.zero;

    //PATHFOLLOWING
    int totalWaypoints;
    int startIndex;

    //LoopPath

    //Wall Avoidance
    RaycastHit hit;


    //Separation
    Vector3 SteeringForce = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        vehicle = GetComponent<Vehicle>();
       
        //Set an initial wander target
        WanderTarget = new Vector3(Random.Range(-WanderRadius, WanderRadius), 0, Random.Range(-WanderRadius, WanderRadius));
    }

    // Update is called once per frame
    void Update()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent");        //get agents
        
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");  //get obstacles
        //check if the AI is crowd or hostile
        if (isCrowd == false)
        {
            localWaypoints = npc.pathCode.pathFindCode.FindPath(transform.position, globalWaypoints[globalIndex].transform.position);   //calculate local waypoints

            pathToTarget = npc.pathCode.pathFindCode.FindPath(transform.position, pathOnTarget);    //calculate local waypoints
            pathOnTarget = npc.targetPos.transform.position;
        }
        else
        {
            pathToTarget = crowdNPC.pathCode.pathFindCode.FindPath(transform.position, pathOnTarget);  //calculate local waypoints
            pathOnTarget = crowdNPC.targetPos.transform.position; //set target
        }

        
        if (pathOnTarget==null)
        {
            pathOnTarget = Vector3.zero;
        }

        

        GetNeighbourhood();
    }

    bool checkClosest = false;
    
    //calculate total force 
    public Vector3 Calculate()
    {
        //Weight Truncated Running Sum with Prioritization
        Vector3 force;
        VelocitySum = Vector3.zero;

        //check if separation is on
        if (IsSeparationOn)
        {
            //calculate separation force
            force = Separation() * separationWeihgt;

            //check if separation force can be added to total force
            if (!accumulatedForce(ref VelocitySum, force))
                return VelocitySum;
        }

        //check if wall avoidance is on
        if (iswallAvoid)
        {
            //calculate wall avoidance force
            force = Wall() * wallAvoidanceWeight;

            //check if wall avoidance force can be added to total force
            if (!accumulatedForce(ref VelocitySum, force))
                return VelocitySum;
        }
       
        //check if alignment is on
        if (IsAlingmentOn)
        {
            //calculate alignment force
            force = Alignment() * alignmentWeight;

            //check if alignment force can be added
            if (!accumulatedForce(ref VelocitySum, force))
                return VelocitySum;
        }

        //check if cohesion is on
        if (isCohesionOn)
        {
            //calculate cohesion force
            force = Cohesion() * cohesionWeight;

            //check if cohesion force can be added
            if (!accumulatedForce(ref VelocitySum, force))
                return VelocitySum;
        }

        //check if seek is on
        if (IsSeekOn)
        {
            //calculate seek force
            force = Seek(SeekOnTargetPos) * seekWeight;

            //check if seek force can be added to total force
            if (!accumulatedForce(ref VelocitySum, force))
                return VelocitySum;
        }


        //check if arrive is on
        if (isArriveOn)
        {
            //calculate arrive force
            force = Arrive(ArriveOnTargetPos) * arriveWeight;

            //check if arrive force can be added
            if (!accumulatedForce(ref VelocitySum, force))
                return VelocitySum;
        }
        return VelocitySum;
    }


    //Seek 
    Vector3 Seek(Vector3 TargetPos)
    {
        Vector3 DesiredVelocity = (TargetPos - transform.position).normalized * vehicle.MaxSpeed;

        return (DesiredVelocity - vehicle.Velocity);
    }

    //Flee
    Vector3 Flee(Vector3 TargetPos)
    {
        Vector3 DesiredVelocity = (transform.position - TargetPos).normalized * vehicle.MaxSpeed;

        return (DesiredVelocity - vehicle.Velocity);
    }


    //Wander
    Vector3 Wander()
    {
        WanderTarget += new Vector3(
            Random.Range(-1f, 1f) * WanderJitter,
            0,
            Random.Range(-1f, 1f) * WanderJitter);

        WanderTarget.Normalize();

        WanderTarget *= WanderRadius;

        Vector3 targetLocal = WanderTarget;

        Vector3 targetWorld = transform.position + WanderTarget;

        targetWorld += transform.forward * WanderDistance;

        return (targetWorld - transform.position)*0.05f;
    }

    //Arrive
    Vector3 Arrive(Vector3 TargetPos)
    {

        Vector3 DesiredVelocity;
        Vector3 toTarget = TargetPos - transform.position;
        float slowingDistance = 5.0f;
        float distanceToTarget = toTarget.magnitude;

       
        if (distanceToTarget < slowingDistance)
        {
            vehicle.CurrentSpeed = distanceToTarget / slowingDistance;
            vehicle.CurrentSpeed = Mathf.Clamp(vehicle.CurrentSpeed, 0.0f, vehicle.MaxSpeed);
            
        }

        DesiredVelocity = (toTarget * vehicle.CurrentSpeed) / distanceToTarget;
        return DesiredVelocity - vehicle.Velocity;
    }


    //Hide
    Vector3 Hide(Vector3 enemyPos,int excludePosition)
    {
        //set default values
        float distance = float.MaxValue;
        Vector3 bestHidingSpot = Vector3.zero;
        Vector3 currentPos=Vector3.zero;
        bestHidingSpot = Vector3.zero;
       
          //calculate the closest hiding spot
         for (int i = 0; i < obstacles.Length && i!=excludePosition; i++)
         {
             if (obstacles[i].transform.position != prevSpot)
             {
                    //get object collider
                    SphereCollider b;   
                    b = obstacles[i].GetComponent<SphereCollider>();
                    
                    //set distance from boundary and distance between boundary and AI
                    float distanceFromBoundary = 2.0f;
                    float distanceAway = distanceFromBoundary + 0.5f;

                    
                    Vector3 toObstacle = (obstacles[i].transform.position - enemyPos).normalized;
                    Vector3 hidingSpot = (toObstacle * distanceAway) + obstacles[i].transform.position;

                    //calculate best hiding spot
                    float distanceToHidingSpot = Vector3.Distance(transform.position, hidingSpot);
                    if (distanceToHidingSpot < distance || obstacles[i].transform.position != prevSpot)
                    {
                         //save new hiding spot
                        bestHidingSpot = hidingSpot;
                        prevSpot = obstacles[i].transform.position;
                        
                    }
                } 
        }
      return bestHidingSpot;
    }

    //path following
    Vector3 pathFollowing(int index)
    {
        return (globalWaypoints[index + 1].transform.position);
    }

    //get closest waypoints
    int ClosestWaypoint()
    {
        //set the closest waypoint to be the first waypoint 
        Vector3 setWaypoint = globalWaypoints[0].transform.position;
        
        //calculate the distance btween AI's position and first waypoint
        float minDistance=Vector3.Distance(globalWaypoints[0].transform.position,transform.position);
        int index = 0;

        //for each waypoint compare the distance between AI and waypoint with minDistance
        for (int i=1;i<totalWaypoints;i++)
        {
            float distance = Vector3.Distance(transform.position, globalWaypoints[i].transform.position);
            //if distance is lower then the minDistance
            if (distance<minDistance)
            {
                //set new min distance
                minDistance = distance;
                setWaypoint = globalWaypoints[i].transform.position;
                //save waypoint indexs
                index = i;
            }
        }
        return index;
    }

    //Separation
    public Vector3 Separation()
    {
        //set steering force to a default value
        Vector3 steeringForce = Vector3.zero;
        
        //calculate distance to all agents, exclude itself
        for (int i = 0; i < Neighbour.Count && Neighbour[i].name != this.name ; i++)
        {
            Vector3 ToAgent = Vector3.zero;
            if (Vector3.Distance(transform.position, Neighbour[i].transform.position) < 5.0f)
            {
                ToAgent = transform.position - Neighbour[i].transform.position;
                float Distance = ToAgent.magnitude;
                ToAgent = ToAgent.normalized;
                ToAgent = ToAgent / Distance;
            }

            steeringForce += ToAgent;
        }
        return steeringForce*0.05f;
    }


    //Alignment
    public Vector3 Alignment()
    {
        //set default value for heading vectoe
        Vector3 AverageHeading = Vector3.zero;

        //calculate the total heading of the neighbour and exclude itself 
        for (int i = 0; i < Neighbour.Count && Neighbour[i].name != this.name; i++)
        {
            AverageHeading += Neighbour[i].transform.forward;
        }

        //calculate average heading
        if (Neighbour.Count>0)
        {
            AverageHeading /= Neighbour.Count;
            AverageHeading -= transform.forward;
        }


        return AverageHeading;
    }

    //Cohesion
   public  Vector3 Cohesion()
    {
        //set default values for Average Position and Steering force
        Vector3 AveragePosition = Vector3.zero;
        Vector3 SteeringForce = Vector3.zero;

        //calculate average position of all agents
        for (int i=0;i<agents.Length; i++)
        {
            AveragePosition += agents[i].transform.position;
        }
        
        if (Neighbour.Count>0)
        {
            AveragePosition /= agents.Length;
            //SteeringForce = Seek(AveragePosition);
        }
        return AveragePosition;
    }

    //cohesion calculated on followers (if the respective agent is a crowd leader)
    public Vector3 followersCohesion()
    {
        //set default values for Average Position and Steering force
        Vector3 AveragePosition = Vector3.zero;
        Vector3 SteeringForce = Vector3.zero;
        
        //get the position of followers
        for (int i = 0; i < followers.Length; i++)
        {
            AveragePosition += followers[i].transform.position;
        }
     
        //calculate average position
            AveragePosition /= followers.Length;
            //SteeringForce = Seek(AveragePosition);
        //return average Vector3 position
        return AveragePosition;
    }

    //Wall avoidance
    Vector3 Wall()
    {
        //targetPos = new Vector3(0.0f, 0.0f, 3.0f);

        Vector3 sf = Vector3.zero;
        //cast raycast
        if (Physics.Raycast(transform.position, transform.position + transform.forward * 4.0f, out hit))
        {
            //calcualte penetration distance
            float penetrationDistance = 5.0f - hit.distance;
            //calculate force
            sf = hit.normal * penetrationDistance;
        }
        return sf;
    }



   //SB Ccontrol Functions
    public void SeekOn(Vector3 TargetPos, float StopDistance = 0.01f)
    {
        IsSeekOn = true;
        SeekOnTargetPos = TargetPos;
        SeekOnStopDistance = StopDistance;
    }

    public void WanderOn()
    {
        IsWanderOn = true;
    }

    public void WanderOff()
    {
        IsWanderOn = false;
        vehicle.Velocity = Vector3.zero;
    }

    public void ArriveOn(Vector3 TargetPos, float stopDistance = 0.1f)
    {
        isArriveOn = true;
        ArriveOnTargetPos = TargetPos;
        ArriveOnStopDistance = stopDistance;
    }

    public void FleeOn(Vector3 TargetPos, float StopDistance = 10.0f)
    {
        IsFleeOn = true;
        FleeFromTargetPos = TargetPos;
        FleeFromStopDistance = StopDistance;
    }

    public void HideOn(Vector3 pos)
    {
        isHideOn = true;
        hideFromPos = pos;
    }

    public void PathOn()
    {
        isPathOn = true;
    }

    public void WallOn()
    {
        iswallAvoid = true;
    }

    public void WallOff()
    {
        iswallAvoid = false;
        vehicle.Velocity = Vector3.zero;
    }

    public void SeparateOn()
    {
        IsSeparationOn = true;
    }

    public void AlignmentOn()
    {
        IsAlingmentOn = true;
    }

    public void CohesionOn()
    {
        isCohesionOn = true;
    }

    //Get neighbours around the agent
    void GetNeighbourhood()
    {
        Neighbour.Clear();
       
        for (int i=0;i<agents.Length; i++)
        {
            //get neighbours that are in range
            if (Vector3.Distance(transform.position,agents[i].transform.position)<10.0f)
            {
                Neighbour.Add(agents[i]);
               // sizeOfNeigbour++;
            }
        }
    }
    
    
    bool accumulatedForce(ref Vector3 Total, Vector3 forceToAdd)
    {
        //calculate the force accumulated 
        float magnitudeSoFar = Total.magnitude;

        //calculate the force that can still be added
        float magnitudeRemaining = vehicle.MaxForce - magnitudeSoFar;

        //if force cannot be added stop
        if (magnitudeRemaining <= 0)
        {
            return false;
        }
        else
        //if force can be added, add force
        {
            Total += forceToAdd.normalized * magnitudeRemaining;
        }
        return true;
    }

    public void LoopPath()
    {
            //disalbe arrive behavior if it was on
            isArriveOn = false;

        //check if global index or local index hasn't been set
        //also check if the local index is equal to the total path length
            if (localIndex==localWaypoints.Length || globalIndex==-1 || localIndex==-1)
        {
            //update global index
            globalIndex = (globalIndex + 1) % globalWaypoints.Count;
            //reset local index
            localIndex = 0;
            //calculate local path
            localWaypoints = npc.pathCode.pathFindCode.FindPath(transform.position,globalWaypoints[globalIndex].transform.position);
        }
            //calculate distance to the local index
            if (Vector3.Distance(transform.position, localWaypoints[localIndex]) < 0.1f)
            {
                 //if distance is lower then a set value, move to the next local waypoint
                localIndex = (localIndex + 1) % (localWaypoints.Length + 1);
            }

            //seek on the local waypoint
            SeekOnTargetPos = localWaypoints[localIndex];
        
    }

    //used to reset the path when target is changed
    public bool resetPathToArrive = true;

    public void pathTarget()
    {
        //activate separation && alignment
        IsSeparationOn = true;
        isArriveOn = true;

        //reset path
            if (resetPathToArrive == true)
            {
                //reset target index, calculate path to target, seek to the first path index
                targetIndex = 0;
                pathToTarget = npc.pathCode.pathFindCode.FindPath(transform.position, pathOnTarget);
                SeekOnTargetPos = pathToTarget[targetIndex];
                IsSeekOn = true;
                
                //disable arrive behavior if it was turned on
                isArriveOn = false;
                resetPathToArrive = false;
            }

            //check if path exists
            if (pathToTarget.Length != 0)
            {
            //check if the AI is close to the next waypoint
            //also checks if the next waypoint isn't the last one
            if (Vector3.Distance(transform.position, pathToTarget[targetIndex]) < 0.01f && targetIndex < pathToTarget.Length)
            {
                //calculate path to the next wayoint
                pathToTarget = npc.pathCode.pathFindCode.FindPath(transform.position, pathOnTarget);
                //reset path index
                targetIndex = 0;
                targetIndex = targetIndex + 1;

            }

            //if waypoint's index is equal to the length of the path or the path does not exist
            if (targetIndex == pathToTarget.Length || pathToTarget.Length == 0)
            {
                //stop the vehicle
                IsSeekOn = false;
                isArriveOn = false;
                vehicle.Velocity = Vector3.zero;

                
                if (pathToTarget.Length != 0)
                    resetPathToArrive = true;
            }
            //if there is only one waypoint left to the final position use the arrive behavior
            else if (pathToTarget.Length==1 && Vector3.Distance(transform.position,pathOnTarget)<5.0f)
            {
                IsSeekOn = false;
                isArriveOn = true;
                ArriveOnTargetPos= pathToTarget[targetIndex];
            }
            //if not use the seek behavior
            else
            {
                IsSeekOn = true;
                isArriveOn = false;
                SeekOnTargetPos = pathToTarget[targetIndex];
            }
        }
    }

    //this function is simillar with the one above but it is usin crowdNPC instead of NPC
    
    public void pathTargetCrowd()
    {
        IsSeparationOn = true;

        if (resetPathToArrive == true)
        {

            targetIndex = 0;
            pathToTarget = crowdNPC.pathCode.pathFindCode.FindPath(transform.position, pathOnTarget);
            SeekOnTargetPos = pathToTarget[targetIndex];
            IsSeekOn = true;
            isArriveOn = false;
            resetPathToArrive = false;
        }
        if (pathToTarget.Length != 0)
        {
            if (Vector3.Distance(transform.position, pathToTarget[targetIndex]) < 0.01f && targetIndex < pathToTarget.Length)
            {
                pathToTarget = crowdNPC.pathCode.pathFindCode.FindPath(transform.position, pathOnTarget);
                targetIndex = 0;
                targetIndex = targetIndex + 1;

            }


            if (targetIndex == pathToTarget.Length || pathToTarget.Length == 0)
            {
                IsSeekOn = false;
                isArriveOn = false;
                vehicle.Velocity = Vector3.zero;

                if (pathToTarget.Length != 0)
                    resetPathToArrive = true;
            }

            else if (pathToTarget.Length == 1 && Vector3.Distance(transform.position, pathOnTarget) < 5.0f)
            {
                IsSeekOn = false;
                isArriveOn = true;
                ArriveOnTargetPos = pathToTarget[targetIndex];
            }
            else
            {
                IsSeekOn = true;
                isArriveOn = false;
                SeekOnTargetPos = pathToTarget[targetIndex];
            }
        }
    }
}
