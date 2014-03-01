using UnityEngine;
using System.Collections;

public class randomMovement : MonoBehaviour {
	Vector3 torque;
	// Use this for initialization
	void Start () {
		torque.x = Random.Range (-2000, 2000);

		torque.z = Random.Range (-2000, 2000);
		constantForce.torque = torque;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
