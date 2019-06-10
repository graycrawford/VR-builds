using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Communicate))]
public class VisualiseRecvTexture : MonoBehaviour
{
    public Renderer targetObj;
    public string materialProperty = "_MainTex";
    Communicate com;
    // Start is called before the first frame update
    void Start()
    {
        com = GetComponent<Communicate>();
    }

    // Update is called once per frame
    void Update()
    {
        targetObj.sharedMaterial.SetTexture(materialProperty, com.output_texture);
        targetObj.sharedMaterial.SetTexture("_EmissiveColorMap", com.output_texture);
    }
}
