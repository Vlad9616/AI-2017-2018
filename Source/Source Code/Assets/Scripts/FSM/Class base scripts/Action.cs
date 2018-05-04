using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Action base clas
public abstract class Action : ScriptableObject
{
    public abstract void Act(StateController controller);
}
