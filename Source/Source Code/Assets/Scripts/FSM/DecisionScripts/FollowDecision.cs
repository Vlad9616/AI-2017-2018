using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/FolloDecision")]
public class FollowDecision : StateDecision
{
    public override bool Decide(StateController controller)
    {
        return CheckFollow(controller);
    }

    public bool CheckFollow(StateController controller)
    {
        
        if (Vector3.Distance(controller.transform.position,controller.crowdNPC.leader.transform.position)>3.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
        
        return false;
    }
	
}
