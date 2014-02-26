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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000))
			{
				Debug.DrawLine(ray.origin, hit.point);
				Debug.Log(hit.triangleIndex);
				waveScript.disturbPoint(hit.point,hit.triangleIndex);
				//waveScript.disturbRandom();
			}
		}
	}

}
