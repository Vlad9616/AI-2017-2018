using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/AmmoAction")]
public class AmmoAction : Action
{

    //Override Act function
    public override void Act(StateController controller)
    {
        Ammo(controller);
    }


    public void Ammo(StateController controller)
    {
        //sets the path target 
        controller.npc.targetPos = controller.npc.closestAmmo;

        //path to the ammo pack
        controller.npc.behaviors.pathTarget();
    }
}
