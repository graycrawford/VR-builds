using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inheritPinch : MonoBehaviour
{

    public Transform thumb;
    public Transform index;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (thumb.position + index.position) / 2;
    }
}
