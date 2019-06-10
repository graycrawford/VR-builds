using UnityEngine;
using System.Collections;

public class SendPositionADifferentWay : MonoBehaviour {

	public OSC osc;
  SphereCollider _collider;
  public float radius = 0.15f;
    public Transform sphere;

  private bool firstCollision = true;

	// Use this for initialization
	void Start () 
  {
    _collider = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () 
  {
    // var colliders = Physics.OverlapSphere(transform.position, );
    if (Physics.CheckSphere(sphere.position, radius))            // transform.position, radius
        {
      if (firstCollision == true)
      {
        Debug.Log("AA");
        
        OscMessage message = new OscMessage();
        message.address = "/";
        message.values.Add("collided");
        osc.Send(message);

        firstCollision = false;
      }
    }
    else 
    {
      firstCollision = true;
    }
  }
  
}
