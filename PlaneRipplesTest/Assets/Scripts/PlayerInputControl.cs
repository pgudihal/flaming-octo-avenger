using UnityEngine;
using System.Collections;

public class PlayerInputControl : MonoBehaviour 
{
	private WavesHandler waveScript;

	// Use this for initialization
	void Start () 
	{
		waveScript = GameObject.Find("WavePlane").GetComponent<WavesHandler>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButton(0))
		{

		}
	}

}
