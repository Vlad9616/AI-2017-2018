using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/Actions/TestAction")]
public class TestAction : Action
{

    public override void Act(StateController controller)
    {
        Test(controller);
    }

    public void Test(StateController controller)
    {
            controller.npc.targetPos = controller.npc.closestHealth;
            controller.npc.behaviors.pathTarget();
 
    }
}
