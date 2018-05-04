using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/PatrolAction")]
public class PatrolAction : Action
{
    //Override Act function
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    public void Patrol(StateController controller)
    {
        //move between waypoints
        controller.npc.behaviors.LoopPath();
    }
}
