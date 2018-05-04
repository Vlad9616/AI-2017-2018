using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/HealthAction")]
public class HealthAction : Action
{
    //Override Act function
    public override void Act(StateController controller)
    {
        GetHealth(controller);
    }


    public void GetHealth(StateController controller)
    {
        //sets the path target
        controller.npc.targetPos = controller.npc.closestHealth;

        //path to the closest health pack
        controller.npc.behaviors.pathTarget();
    }
}

