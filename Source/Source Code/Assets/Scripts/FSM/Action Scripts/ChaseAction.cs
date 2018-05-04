using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/ChaseAction")]
public class ChaseAction : Action
{
    //override Act function
    public override void Act(StateController controller)
    {
        Chase(controller);
    }

    public void Chase(StateController controller)
    {
        //set chase target
        controller.npc.targetPos = controller.npc.player;
        
        //path to chase target
        controller.npc.behaviors.pathTarget();
    }
}

