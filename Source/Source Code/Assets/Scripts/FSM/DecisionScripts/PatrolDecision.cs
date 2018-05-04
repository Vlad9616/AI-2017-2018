using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/PatrolDecision")]
public class PatrolDecision : StateDecision
{
    //Override Decide Function
    public override bool Decide(StateController controller)
    {
        return CheckPatrol(controller);
    }

    //check if the AI can see the player, detected any noise, health level, ammo level

    public bool CheckPatrol(StateController controller)
    {
        if (controller.npc.canSee == false && controller.npc.canHearPlayer==false && controller.npc.healthDesire <0.5f && controller.npc.ammoDesire < 0.5f)
        {

            controller.npc.behaviors.IsSeekOn = true;
            return true;
        }
        else
        {
            controller.npc.behaviors.IsSeekOn = false;
            return false;
        }
    }
}
