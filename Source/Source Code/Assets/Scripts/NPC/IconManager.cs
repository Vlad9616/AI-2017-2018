using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{

    public int currentIcon = 0; //
    public State currentState;  //AI's current state
    public StateController controller;  //get access to the state controller
    public bool iconOn;     //enable/disable icons

    public GameObject[] spriteList; //list of all 2d icons

	// Use this for initialization
	void Start ()
    {
        //get state controller
        controller = GetComponent<StateController>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //keep current state and icon updated
        currentState = controller.currentState;
        currentIcon = controller.currentState.index;
        if (iconOn == true)
            SelectIcon(currentIcon);
        else
            IconOff();
    }

    //check which icon is set to active and should be displayed
    void SelectIcon(int index)
    {
        for (int i=0;i<spriteList.Length;i++)
        {
            if (i==currentIcon)
            {
                spriteList[i].SetActive(true);
            }
            else
            {
                spriteList[i].SetActive(false);
            }
        }
    }

    void IconOff()
    {
        for (int i = 0; i < spriteList.Length; i++)
            spriteList[i].SetActive(false);
    }
}
