using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LASPtranslate : MonoBehaviour
{
    public float input { get; set; }

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(0.0f, input, 0.0f);

        // self.position.y = input;
        //spherePosition = new Vector3(2.0f * Mathf.Sin(xzPosition), 4.0f * Mathf.Sin(yPosition), 2.0f * Mathf.Cos(xzPosition));
        //transform.position = spherePosition;
    }
}
