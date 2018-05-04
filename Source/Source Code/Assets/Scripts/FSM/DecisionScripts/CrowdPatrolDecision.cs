using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/CrowdPatrolDecision")]
public class CrowdPatrolDecision : StateDecision
{
    //Override Decide function
    public override bool Decide(StateController controller)
    {
        return CheckPatrol(controller);
    }

    public bool CheckPatrol(StateController controller)
    {
       //check distance to the average position of the followers
         if (Vector3.Distance(controller.npc.transform.position, controller.npc.behaviors.followersCohesion())<5.0f)
        {

            controller.npc.behaviors.IsSeekOn = true;
            return true;
        }
        else
        {
          controller.npc.behaviors.IsSeekOn = false;
           controller.npc.behaviors.VelocitySum = Vector3.zero;
           controller.npc.behaviors.vehicle.Velocity = Vector3.zero;
           return false;
        }
    }
}
