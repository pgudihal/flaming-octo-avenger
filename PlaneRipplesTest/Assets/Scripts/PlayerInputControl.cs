using UnityEngine;
using System.Collections;

public class PlayerInputControl : MonoBehaviour 
{
	private WavesHandler waveScript;
	private GameObject playerObj;
	public GameObject defaultObj;

	// Use this for initialization
	void Start () 
	{
		waveScript = GameObject.Find("WavePlane").GetComponent<WavesHandler>();
		playerObj = GameObject.Find ("PlayerObj");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		if(Input.GetMouseButton(0) || !defaultObj.rigidbody.IsSleeping())
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 trailingPos = Camera.main.WorldToScreenPoint(defaultObj.transform.position);
			Ray ray2 = Camera.main.ScreenPointToRay(trailingPos);
			RaycastHit hit, hit2;
			if (Physics.Raycast(ray, out hit, 1000))
			{
				Vector3 playerPos = hit.point;
				playerPos.y = 1f;
				this.transform.position = playerPos;
				Debug.DrawLine(ray.origin, hit.point);
				if(Physics.Raycast(ray2,out hit2, 1000)){
					waveScript.disturbPoint(hit2.point,hit2.triangleIndex);
				}
				//waveScript.disturbRandom();
			}
		}
	}

}
