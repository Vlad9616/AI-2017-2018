using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/NoiseDecision")]
public class NoiseDecision : StateDecision
{
    //Ovveride Decide Function
    public override bool Decide(StateController controller)
    {
        return CheckSound(controller);
    }

    
    public bool CheckSound(StateController controller)
    {
        


        if (controller.npc.playerCode.soundObj != null)
        {
            //check if the AI can see the player,hear the player or if the AI reached the sound source position
            //check AI's health and ammo levels
            if (controller.npc.canSee == false && (controller.npc.canHearPlayer == true || controller.npc.soundPositionReached == false) && controller.npc.healthDesire < 0.5f && controller.npc.ammoDesire < 0.5f)
            {
                //check distance to the sound source in order to see if the AI reached the soundsource
                if (Vector3.Distance(controller.npc.transform.position, controller.npc.playerCode.soundObj.transform.position) > 2.0f)
                {
                    return true;
                }
                else
                {
                    //AI reached the sound source
                    controller.npc.soundPositionReached = true;
                    return false;
                }
            }
            else
                return false;
        }
        return false;
    }
}
