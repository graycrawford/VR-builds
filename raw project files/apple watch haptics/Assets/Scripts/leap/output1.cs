using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Modes
{
    Mode1,
    Mode2
}

public class output1 : MonoBehaviour
{

    public Transform fingertipL;
    public Transform fingertipR;

    public Transform thumbtipL;
    public Transform thumbtipR;

    public Transform palmL;
    public Transform palmR;
    public Transform leap;

    public Quaternion handRotStarting;

    public float leapToLFinger;
    public float leapToRFinger;
    public float leapToPalmL;

    // translation
    public float RPalmX;
    public float RPalmY;
    public float RPalmZ;
    public float LPalmX;
    public float LPalmY;
    public float LPalmZ;

    // rotation
    public float RPalmRotZ;


    public int mode;

    // scale factors
    public float Xscale = 1.0f;
    public float Yscale = 1.0f;
    public float Zscale = 1.0f;

    public float Xoffset = 0.0f;
    public float Yoffset = 0.0f;
    public float Zoffset = 0.0f;

    public float rotScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        // distance between fingers
        Debug.DrawLine(fingertipR.position, fingertipL.position, Color.white);
        float fingerDistance = (fingertipR.position - fingertipL.position).magnitude;

        // pinch point location
        Vector3 pinchPointL = (fingertipL.position + thumbtipL.position)/2;
        Vector3 pinchPointR = (fingertipR.position + thumbtipR.position)/2;
        Debug.Log("pinchL" + pinchPointL);
        Debug.Log("pinchR" + pinchPointR);


        // distance from rig
        Debug.DrawLine(leap.position, fingertipL.position, Color.white);
        Debug.DrawLine(leap.position, fingertipR.position, Color.white);
        leapToLFinger = (leap.position - fingertipL.position).magnitude;
        leapToRFinger = (leap.position - fingertipR.position).magnitude;

        // leapToPalmL = Mathf.Abs((leap.position.y - palmL.position.y)) * scale;



        // translation
        LPalmX = (Mathf.Abs((leap.position.x - palmL.position.x)) + Xoffset) * Xscale;
        LPalmY = (Mathf.Abs((leap.position.y - palmL.position.y)) + Yoffset) * Yscale;
        LPalmZ = (Mathf.Abs((leap.position.z - palmL.position.z)) + Zoffset) * Zscale;

        RPalmX = (Mathf.Abs((leap.position.x - palmR.position.x)) + Xoffset) * Xscale;
        RPalmY = (Mathf.Abs((leap.position.y - palmR.position.y)) + Yoffset) * Yscale;
        RPalmZ = (Mathf.Abs((leap.position.z - palmR.position.z)) + Zoffset) * Zscale;



        RPalmRotZ = (palmR.rotation.eulerAngles.z - 90) * rotScale;



        //Debug.Log("fingerDistance " + fingerDistance);
        //Debug.Log("leapToLFinger  " + leapToLFinger);
        //Debug.Log("leapToRFinger  " + leapToRFinger);
        // Debug.Log("L palm height  " + leapToRFinger);
        Debug.Log("L " + LPalmX + LPalmY + LPalmZ);
        Debug.Log("R " + RPalmX + RPalmY + RPalmZ);

        //Debug.Log("R palm Zrot    " + RPalmRotZ);

        // Debug.Log(fingertipL.transform.position);
    }
}