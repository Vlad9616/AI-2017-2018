//Unity. (n.d). Finite State AI with the Delegate Pattern. Retrieved from: https://unity3d.com/learn/tutorials/topics/navigation/finite-state-ai-delegate-pattern

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State Decision base clase
[CreateAssetMenu(menuName = "AI/Decisions/Idle")]
public abstract class StateDecision : ScriptableObject
{
    public abstract bool Decide(StateController controller);
}
