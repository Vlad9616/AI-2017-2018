//Unity. (n.d). Finite State AI with the Delegate Pattern. Retrieved from: https://unity3d.com/learn/tutorials/topics/navigation/finite-state-ai-delegate-pattern
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateTransition 
{
    public StateDecision decision;  //state decision
    public State nextState;         //state if decision result is positive
    public State lastState;         //state if decision result is negative

}
