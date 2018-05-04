using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/AmmoDecision")]
public class AmmoDecision : StateDecision
{
    //Override Decide Function
    public override bool Decide(StateController controller)
    {
        return CheckAmmo(controller);
    }

    public bool CheckAmmo(StateController controller)
    {
        //checl health level
        if (controller.npc.ammoDesire>0.5f)
            return true;
        else
            return false;
    }
}

