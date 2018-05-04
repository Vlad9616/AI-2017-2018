using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/FollowAction")]
public class FollowAction : Action 
{
    //Override Act fucntion
    public override void Act(StateController controller)
    {
        FollowLeader(controller);
    }

    public void FollowLeader(StateController controller)
    {   //set target position
        controller.crowdNPC.targetPos = controller.crowdNPC.leader;

        //path to target
        controller.crowdNPC.behaviors.pathTargetCrowd();
    }
}
