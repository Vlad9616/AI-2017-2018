using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/ChaseNoise")]
public class ChaseNoise : Action
{
    //Override Act function
    public override void Act(StateController controller)
    {
        GetNoise(controller);
    }

    public void GetNoise(StateController controller)
    {
        //set the path target 
        controller.npc.targetPos = controller.npc.playerCode.soundObj;

        //path to the sound source object
        controller.npc.behaviors.pathTarget();
    }
}
