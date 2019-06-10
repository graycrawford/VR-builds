using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct Operations
{
    public Operation[] ops;
}


[System.Serializable]
public struct Operation
{
    public string categoryOrZ; // category, z
    public string op; // add, sub, slerp, randomize, set, zero, setAsVisualize, setTruncation, resets
    public string left; // name of z vector or category
    public string right; // name of z vector or category, index of position if set
    public string t; // slerp amount, truncation value, set value
    public string result; // result vector name, created if non extant

    public static implicit operator Operations(Operation op) {
        return new Operations {
            ops = new Operation[] { op }
        };
    }

    // create a new vector named random0 with truncation = 0.5
    public static Operation RandomizeExample = new Operation {
        categoryOrZ = "z",
        op = "randomize",
        t = "0.5",
        result = "random"
    };

    // create a new category vector named x initalized to 0
    public static Operation ZeroExample = new Operation {
        categoryOrZ = "category",
        op = "zero",
        result = "x"
    };

    // set the truncation sent to the model to 0.5
    public static Operation SetTruncationExample = new Operation
    {
        categoryOrZ = "z",
        op = "setTruncation",
        t = "0.5",
    };
    // set the truncation sent to the model to 0.5
    public static Operation SetTruncationByNameExample = new Operation
    {
        categoryOrZ = "z",
        op = "setTruncation",
        result = "underwaterExplosion1"
    };

    // add two z vectors init and random0
    public static Operation AddExample = new Operation {
        categoryOrZ = "z",
        op = "add",
        left = "init",
        right = "random0",
        result = "initPlusRandom0"
    };

    // interpolate between two z vectors init & random0
    public static Operation SlerpExample = new Operation {
        categoryOrZ = "z",
        op = "slerp",
        left = "init",
        right = "random0",
        t = "0.5",
        result = "initSlerpRandom0"
    };

    // this sets init[5] = 1, which for categories is equivalent to
    // mixing in a new category (at index 5) with weight 1
    public static Operation SetExample = new Operation {
        categoryOrZ = "category",
        op = "set",
        left = "init",
        right = "5",
        t = "1",
        result = "mixCategory"
    };

}

#if UNITY_EDITOR
[CustomEditor(typeof(Interpreter))]
public class InterpreterInspector : Editor
{
    public override void OnInspectorGUI() {
        var interpreter = (Interpreter)target;
        GUILayout.Label(interpreter.isValid ? "ok" : interpreter.err);
        GUI.enabled = interpreter.isValid;
        if(GUILayout.Button("Execute")) {
            interpreter.ExecuteGUI();
        }
        GUI.enabled = true;
        DrawDefaultInspector();
    }
}
#endif 

[RequireComponent(typeof(Communicate))]
public class Interpreter : MonoBehaviour
{
    public Operation manualOperation;

    public Transform zHolder;
    public Transform catHolder;
    public Transform vectorProto;
    public List<string> preloadedVectors = new List<string>();
    Communicate comm;

    public List<string> zNames = new List<string>();
    public List<string> catNames = new List<string>();
    public bool isValid = true;
    [HideInInspector]
    public string err = "";
    public Queue<Operation> queue = new Queue<Operation>();

    private void OnValidate() {
        isValid = Validate(manualOperation);
    }

    // Start is called before the first frame update
    void Start()
    {
        comm = GetComponent<Communicate>();
        // server performs these at start
        foreach(var vecName in preloadedVectors) {
            SyncUIToOp(new Operation { op = "zero", result = vecName, categoryOrZ = "category" });
            SyncUIToOp(new Operation { op = "zero", result = vecName, categoryOrZ = "z" });
        }
    }
    
    List<string> validOps = new List<string> {
        "add", "sub", "slerp", "randomize", "set", "zero", "setAsVisualize", "setTruncation", "nop", "reset"
    };

    bool Validate(Operation op) {
        var listToUpdate = op.categoryOrZ == "z" ? zNames : catNames;
        if (op.op == "nop" || op.op == "reset") return true;

        if (!validOps.Contains(op.op)) {
            err = "invalid op";
            return false;
        }
        if(op.categoryOrZ != "z" && op.categoryOrZ != "category") {
            err = "invalid valid for categoryOrZ";
            return false;
        }
        if (op.op == "add" || op.op == "sub" || op.op == "slerp") {
            if (!listToUpdate.Contains(op.left) || !listToUpdate.Contains(op.right)) {
                err = ("Invalid argument for op.left or op.right");
                return false;
            }
            if (op.op == "slerp" && op.t == "") {
                err = ("No value given for scalar t paramenter");
                return false;
            }
        }
        //|| 
        if((op.op == "set") && !listToUpdate.Contains(op.left)) {
            Debug.LogError(JsonUtility.ToJson(op));
            Debug.LogError(listToUpdate.ToString());
            err = ("Invalid argument for op.left");
            return false;
        }
        if (op.op == "set" && op.right == "") {
            err = ("No value given for index for set");
            return false;
        }
        if (op.op == "set" && System.Int32.TryParse(op.right, out int idx)) {
            if (op.categoryOrZ == "z" && (idx < 0 || idx >= 128)) {
                err = ("Idx for set out of range");
                return false;
            }
            else if ((idx < 0 || idx >= 1000)) {
                err = ("Idx for set out of range");
                return false;
            }
        }

        if (op.op == "set" || op.op == "randomize") {
            if(op.t == "") {
                err = ("No value for scalar parameter t");
                return false;
            }
        }

        if(op.op == "setTruncation")
        {
            if(op.t == "" && !listToUpdate.Contains(op.result))
            {
                Debug.LogError(listToUpdate);
                err = "must specify a truncation value in parameter op.t or a preset truncation name in op.result";
                return false;
            }
        }
        if(op.result == "") {
            err = ("No result name");
            return false;
        }

        err = "";
        return true;
    }

    public void ExecuteGUI() {
        comm.Send(JsonUtility.ToJson(manualOperation), () => SyncUIToOp(manualOperation));
    }

    public void SyncUIToOp(Operations ops) {
        foreach(var op in ops.ops) {
            SyncUIToOp(op);
        }
    }

    public void SyncUIToOp(Operation op) {
        var list = op.categoryOrZ == "z" ? zNames : catNames;
        var holder = op.categoryOrZ == "z" ? zHolder : catHolder;
        if (!list.Contains(op.result) && op.op != "setTruncation") {
            list.Add(op.result);
            var rep = Instantiate(vectorProto, holder);
            rep.name = op.result;
        }
    }


    public bool Execute(Operations ops, bool queue = false, System.Action cb = null) {
        foreach (var op in ops.ops) {
            var listToUpdate = op.categoryOrZ == "z" ? zNames : catNames;
            if (!Validate(op)) {
                Debug.LogError(string.Format("invalid op: {0} {1}", JsonUtility.ToJson(op), err));
                return false;
            }
        }
        return comm.Send(JsonUtility.ToJson(ops), () => {
            SyncUIToOp(ops); if (cb != null) cb();
        }, queue);
    }
}
