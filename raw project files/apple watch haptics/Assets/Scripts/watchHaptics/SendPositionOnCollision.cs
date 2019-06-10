using UnityEngine;
using System.Collections;

public class SendPositionOnCollision : MonoBehaviour {

	public OSC osc;

	// Use this for initialization
	void Start () 
  {

	}
	
	// Update is called once per frame
	void Update () 
  {

  }

  void OnCollisionEnter(Collision collision)
  {
    OscMessage message = new OscMessage();
    message.address = "/";
    // message.values.Add("collided!");
    message.values.Add(collision.relativeVelocity.magnitude);
    osc.Send(message);
  }


}
