using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

    /////////////////////
    //Updated Values
    /////////////////////
    /// <summary>
    /// This is applied to the current position every frame
    /// </summary>
    public Vector3 Velocity;

    //Position, Heading and Side can be accessed from the transform component with transform.position, transform.forward and transform.right respectively

    //"Constant" values, they are public so we can adjust them through the editor

    //Represents the weight of an object, will effect its acceleration
    public float Mass = 100;

    //The maximum speed this agent can move per second
    public float MaxSpeed = 1.0f;
    public float CurrentSpeed = 0.5f;

    //The thrust this agent can produce
    public float MaxForce = 30;

    //We use this to determine how fast the agent can turn, but just ignore it for, we won't be using it
    public float MaxTurnRate = 50.0f;


    private SteeringBehaviours SB;

    // Use this for initialization
    void Start ()
    {
        SB = GetComponent<SteeringBehaviours>();
	}
	
    public Vector3 CurrentSteeringForce=Vector3.zero;
	// Update is called once per frame
	void Update ()
    {
        Vector3 SteeringForce = SB.Calculate();
        SteeringForce = Vector3.ClampMagnitude(SteeringForce, MaxForce);


        Vector3 Acceleration = SteeringForce / Mass;
        
        Velocity += Acceleration;

        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        if (Velocity != Vector3.zero)
        {
            transform.position += Velocity * Time.deltaTime;

            transform.forward = Velocity.normalized;
        }

        //transform.right should update on its own once we update the transform.forward
	}
}
