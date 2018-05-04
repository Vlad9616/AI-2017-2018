using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/FireAction")]
public class FireAction : Action
{
    //override Act functio
    public override void Act(StateController controller)
    {
        Fire(controller);
    }

    public void Fire(StateController controller)
    {
        //check distance to player
        if (Vector3.Distance(controller.transform.position, controller.npc.player.transform.position) < 5.0f)
        {
            //shoot player
            controller.npc.shoot();
        }
    }
}
