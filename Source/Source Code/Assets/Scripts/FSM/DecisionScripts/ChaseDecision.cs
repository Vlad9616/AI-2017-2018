using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/ChaseDecision")]
public class ChaseDecision : StateDecision
{
    //Override Decide function
    public override bool Decide(StateController controller)
    {
        return CheckChase(controller);
    }

    public bool CheckChase(StateController controller)
    {
        //check if AI can see the player, has enough health and ammo
        if ((controller.npc.canSee == true ||  controller.npc.isHit == true) && controller.npc.healthDesire < 0.5f && controller.npc.ammoDesire < 0.5f)
       

            return true;
            
        else

            return false;
        
    }
}
