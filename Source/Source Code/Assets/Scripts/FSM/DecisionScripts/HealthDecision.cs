using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/HealthDecision")]
public class HealthDecision : StateDecision
{
    //Override Decide function
    public override bool Decide(StateController controller)
    {
        return CheckHealth(controller);
    }

    //check the health and ammo status
    public bool CheckHealth(StateController controller)
    {
        if (controller.npc.healthDesire >= 0.5f && controller.npc.ammoDesire<0.5f)
            return true;
        else
            return false;
    }
}
