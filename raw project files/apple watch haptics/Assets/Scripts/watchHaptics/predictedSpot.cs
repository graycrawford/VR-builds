using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predictedSpot : MonoBehaviour
{
    public Rigidbody fingertip;
    public float scale = 1.0f;
    private Vector3 predictedLocation;
    private Vector3 fingertipVelocity;

    private Vector3 pos;

    private Vector3 emaVelocity = Vector3.zero;
    public float ema = 0.01f;

    void Start()
    {
        pos = fingertip.position;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        fingertipVelocity = (fingertip.position - pos) / Time.fixedDeltaTime;
        pos = fingertip.position;     
        emaVelocity = fingertipVelocity * ema + (1.0f - ema) * emaVelocity;
        predictedLocation = fingertip.position + (emaVelocity * scale);
        transform.position = predictedLocation;
        //Debug.Log(fingertipVelocity);

             
        // fingertipVelocity = fingertip.velocity;    
        //Debug.Log("fingertip " + fingertip.position);
        //Debug.Log("predicted " + predictedLocation);
    }

}

//float distanceTravelled = 0;
//Vector3 lastPosition;


















// get fingertip velocity and vector



// var secondsInAdvance : float;
// var framesInAdvance : int;

// var useFrameCounter : Boolean;

// function CalcFuturePos () : Vector3 {
//     var finalPos : Vector3 = transform.position;
//     var velocity = rigidbody.velocity;

//  if(useFrameCounter) {
//      velocity *= Time.deltaTime;
//      velocity *= framesInAdvance;
//      finalPos +=  velocity;
//  }
//  else {
//     velocity *= secondsInAdvance;
//     finalPos += velocity;
//  }
//  return finalPos;

// }