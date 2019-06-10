using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interpreter))]
public class SliderSlerp : MonoBehaviour
{

    Interpreter interpreter;

    public GameObject interpolateA;
    public GameObject interpolateB;

    [Range(0.0f, 1.0f)]
    public float slider = 0.0f;

    float prevVal;
    // Start is called before the first frame update
    void Start()
    {
        interpreter = GetComponent<Interpreter>();
    }

    bool first = true;
    bool isReady = true;

    // Update is called once per frame
    void Update()
    {
        if (interpolateA == null || interpolateB == null) return;
        if (isReady && prevVal != slider) {
            prevVal = slider;
            interpreter.Execute(new Operation {
                categoryOrZ = "z",
                op = "slerp",
                left = interpolateA.name,
                right = interpolateB.name,
                t = slider.ToString(),
                result = "visualize"
            }, false, () => isReady = true);
            isReady = false;
            Debug.Log("sending");
        }
    }
}
