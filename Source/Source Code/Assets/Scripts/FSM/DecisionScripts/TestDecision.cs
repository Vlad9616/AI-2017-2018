using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/TestDecision")]
public class TestDecision : StateDecision
{

    public override bool Decide(StateController controller)
    {
        return Test(controller);
    }

    public bool Test(StateController controller)
    {
        return true;
    }
}
