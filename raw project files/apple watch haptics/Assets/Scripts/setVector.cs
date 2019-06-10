using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class setVector : MonoBehaviour
{

    public output1 leapInfo;
    public Interpreter interpret;

    public bool isActive = false;

    public GameObject startLocation;

    public int vectorIndex0 = 0;
    public int vectorIndex1 = 0;
    public int vectorIndex2 = 0;
    public int vectorIndex3 = 0;
    public int vectorIndex4 = 0;
    public int vectorIndex5 = 0;

    public void setIndex0(int i)
    { vectorIndex0 = i; }

    public void setIndex1(int i)
    { vectorIndex1 = i; }

    public void setIndex2(int i)
    { vectorIndex2 = i; }

    public void setIndex3(int i)
    { vectorIndex3 = i; }

    public void setIndex4(int i)
    { vectorIndex4 = i; }

    public void setIndex5(int i)
    { vectorIndex5 = i; }



    public void setActivated(bool value)
    {
        isActive = value;
    }



    // Start is called before the first frame update
    void Start()
    {

    }

    bool first = true;
    float accumulator = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (first)
        {
            interpret.Execute(new Operations
            {
                ops = new Operation[] {
                    new Operation
                        { categoryOrZ = "category",
                            op = "reset"
                        },
                    new Operation
                        { categoryOrZ = "category",
                            op = "setAsVisualize",
                            result = startLocation.name
                        },
                    new Operation
                        { categoryOrZ = "z",
                            op = "setAsVisualize",
                        result = startLocation.name
                    },
                    new Operation
                        { categoryOrZ = "z",
                            op = "setTruncation",
                        result = startLocation.name
                    }
                }
            }, true);

            first = false;
        }

        if (!first && isActive)
        {

            var lX = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex0.ToString(), // (Mathf.Abs((20 * Mathf.FloorToInt(leapInfo.RPalmRotZ / 20.0f)) % 128)).ToString(),
                t = leapInfo.LPalmX.ToString(),
                result = startLocation.name
            };

            var lY = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex1.ToString(), 
                t = leapInfo.LPalmY.ToString(),
                result = startLocation.name
            };

            var lZ = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex2.ToString(),
                t = leapInfo.LPalmZ.ToString(),
                result = startLocation.name
            };

            var rX = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex3.ToString(),
                t = leapInfo.RPalmX.ToString(),
                result = startLocation.name
            };

            var rY = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex4.ToString(),
                t = leapInfo.RPalmY.ToString(),
                result = startLocation.name
            };

            var rZ = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex5.ToString(),
                t = leapInfo.RPalmZ.ToString(),
                result = startLocation.name
            };



            // Debug.Log("lX" + lX.right);
            // Debug.Log("lY" + lY.right);
            // Debug.Log("lZ" + lZ.right);
            // Debug.Log("rX" + rX.right);
            // Debug.Log("rY" + rY.right);
            // Debug.Log("rZ" + rZ.right);




            /*var opRotate = new Operation
            {
                categoryOrZ = "z",
                op = "set",
                left = startLocation.name,
                right = vectorIndex2.ToString(),
                t = leapInfo.RPalmRotZ.ToString(),
                result = startLocation.name
            }; */
            var ops = new Operations { ops = new Operation[] { lX, lY, lZ, rX, rY, rZ } };
            interpret.Execute(ops);

        }

        //    categoryOrZ = "category",
        //    op = "set",
        //    left = "init",
        //    right = "5",
        //    t = "1",
        //    result = "mixCategory"



    }
}





//public string categoryOrZ; // category, z
//public string op; // add, sub, slerp, randomize, set, zero, setAsVisualize, setTruncation
//public string left; // name of z vector or category
//public string right; // name of z vector or category, index of position if set
//public string t; // slerp amount, truncation value, set value
//public string result; // result vector name, created if non extant

//// create a new vector named random0 with truncation = 0.5
//public static Operation RandomizeExample = new Operation
//{
//    categoryOrZ = "z",
//    op = "randomize",
//    t = "0.5",
//    result = "random"
//};

//// create a new category vector named x initalized to 0
//public static Operation ZeroExample = new Operation
//{
//    categoryOrZ = "category",
//    op = "zero",
//    result = "x"
//};

//// set the truncation sent to the model to 0.5
//public static Operation SetTruncationExample = new Operation
//{
//    categoryOrZ = "z",
//    op = "setTruncation",
//    t = "0.5",
//};

//// add two z vectors init and random0
//public static Operation AddExample = new Operation
//{
//    categoryOrZ = "z",
//    op = "add",
//    left = "init",
//    right = "random0",
//    result = "initPlusRandom0"
//};

//// interpolate between two z vectors init & random0
//public static Operation SlerpExample = new Operation
//{
//    categoryOrZ = "z",
//    op = "slerp",
//    left = "init",
//    right = "random0",
//    t = "0.5",
//    result = "initSlerpRandom0"
//};

//// this sets init[5] = 1, which for categories is equivalent to
//// mixing in a new category (at index 5) with weight 1
//public static Operation SetExample = new Operation
//{
//    categoryOrZ = "category",
//    op = "set",
//    left = "init",
//    right = "5",
//    t = "1",
//    result = "mixCategory"
