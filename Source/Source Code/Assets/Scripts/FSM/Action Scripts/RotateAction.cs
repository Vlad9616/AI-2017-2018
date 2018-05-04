using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/RotateAction")]
public class RotateAction : Action
{
    //override Act function
    public override void Act(StateController controller)
    {
        Rotate(controller);
    }


    public void Rotate(StateController controller)
    {
        //check if AI was hit
        if (controller.npc.isHit)
        {
            //stop AI
            controller.npc.behaviors.vehicle.Velocity = Vector3.zero;

            //calculate AI-Player Vecto3
            Vector3 targetDirection = controller.npc.player.transform.position - controller.transform.position;
            float rotationSpeed = 0.01f;

            //Rotate the AI towards the Player
            Vector3 newDirection = Vector3.RotateTowards(controller.transform.forward, targetDirection, rotationSpeed, 5.0f);
            controller.transform.rotation = Quaternion.LookRotation(newDirection);

            //set the cansSee variable to true so the AI will chase the player
            controller.npc.canSee = true;

            //Reset hit variable
            controller.npc.isHit = false;
        }
    }
}
