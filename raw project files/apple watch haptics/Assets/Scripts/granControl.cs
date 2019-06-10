using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;


//[System.Serializable]
//public class FloatEvent : UnityEvent<float>
//{
//}


public class granControl : MonoBehaviour
{
    public Transform hand;

    
    //public FloatEvent evt;
    public Granulator gran;

    public Transform xZero;
    public Transform xOne;
    private float xDifference;
    private float xPos;

    public Transform yZero;
    public Transform yOne;
    private float yDifference;
    private float yPos;
    public float grainPosRandScale = 0.017f; // in ms, scaling 0...1 to default of 50
    public float grainPosRandOffset = 0.003f; // granulator script default sounded good at 50 



    public Transform thumb;
    public Transform index;
    private float pinchDistance;
    public float pinchScale = 5.0f;
    private float ScaledPinch;

    
    // Start is called before the first frame update
    void Start()
    {
        xDifference = xOne.position.x - xZero.position.x;
        yDifference = yOne.position.y - yZero.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        // x
        if (hand.position.x > xZero.position.x && hand.position.x < xOne.position.x)
        {
            xPos = (hand.position.x - xZero.position.x) / xDifference;
            gran.grainPos = xPos;
            //Debug.Log(xPos);
            //gran.grainVol = 
        }

        // y
        if (hand.position.y > yZero.position.y && hand.position.y < yOne.position.y)
        {
            yPos = (hand.position.y - yZero.position.y) / yDifference;
            // gran.grainPosRand = grainPosRandOffset + (yPos * grainPosRandScale); // in  ms
            //Debug.Log(yPos);
            //gran.grainVol = 
        }





        pinchDistance = (thumb.position - index.position).magnitude;

        if (pinchDistance < 0.02f)
        {
            gran.grainVol = 0;
        }
        else
        {
            ScaledPinch = pinchDistance * pinchScale;
            gran.grainVol = Mathf.Clamp(ScaledPinch, 0.0f, 1.0f);
            Debug.Log(gran.grainVol);
        }
        
    }
}
