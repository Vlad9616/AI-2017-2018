//Unity. (n.d). Finite State AI with the Delegate Pattern. Retrieved from: https://unity3d.com/learn/tutorials/topics/navigation/finite-state-ai-delegate-pattern
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class StateController : MonoBehaviour
{
    public NPC npc;     //reference to NPC code
    public CrowdNPC crowdNPC;   //reference to crowd NPC code
   
    public State currentState;      //AI's current state
    public State remainState;
    bool aiActive=true;

    private void Awake()
    {
        npc = GetComponent<NPC>();      //get NPC code
        
    }
    private void Update()
    {
        if (!aiActive)
            return;
        currentState.UpdateStates(this);   
    }

    //check and move to next state
    public void TransitionToState(State nextState)
    {

        if (nextState!=remainState)
        {
            currentState = nextState;
        }
    }
}
