using UnityEngine;
using System.Collections;

public class CustomAssetTester : MonoBehaviour {

	public SwipeSession session;
	// Use this for initialization
	void Start () 
	{
		Debug.Log(session);
		Debug.Log(session.levels.Count);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
