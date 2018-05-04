//Unity. (n.d). Finite State AI with the Delegate Pattern. Retrieved from: https://unity3d.com/learn/tutorials/topics/navigation/finite-state-ai-delegate-pattern
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI/State")]
public class State : ScriptableObject
{
    public Action[] actions;    //actions array
    public StateTransition[] transitions;   //transitions array
    public GameObject Icon;     //state 2d icon
    public int index;   //state index 

    public void UpdateStates(StateController controller)
    {
        DoActions(controller);
        CheckTransition(controller);
    }

    //activate state's actions
    private void DoActions(StateController controller)
    {
        for (int i=0;i<actions.Length;i++)
        {
            actions[i].Act(controller);
        }
    }

    //check state's transitions
    private void CheckTransition(StateController controller)
    {
        for (int i=0;i<transitions.Length;i++)
        {
            bool isSucceeded = transitions[i].decision.Decide(controller); //check transition condition

            //if condition is true, move to next state
            if (isSucceeded)
            {
                controller.TransitionToState(transitions[i].nextState);
            }
            else
            {
                controller.TransitionToState(transitions[i].lastState);
            }
        }
    }
}
