using UnityEngine;
using System.Collections;

public class SwipeLevelManager : MonoBehaviour 
{

	public SwipeSession[] sessions;
	public float HitHeight = 1;
	private float planarRayLength = 10;

	private Vector3[] currentLvlPositions;
	private int currentSession = 0;
	private int currentSessionLvl = 0;
	private Vector3 lastFramePos = new Vector3(0,1,0);
	public ArrowManager arrowManager;

	// Use this for initialization
	void Start () 
	{
		currentLvlPositions = sessions[currentSession].levels[currentSessionLvl].levelData;
		loadCurrentLevel();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(arrowManager.areArrowsReturned()) LoadNextLevel();
		if(Input.GetMouseButton(0)) pressing();
	}

	private void pressing()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000))
		{
			//removing the hit arrow from the initial raycast
			Debug.DrawLine(ray.origin, hit.point);
			if(hit.transform.tag == "Arrow")
				hit.transform.GetComponent<Arrow>().hit();

			//in order to account for fast swipes we can raycast from the last frame's hit
			//point to the current frames hit point. this raycast is parralel to the 
			//plane
			Vector3 hitAdjustedHeight = new Vector3(hit.point.x,1,hit.point.z);

			RaycastHit[] hits;
			hits = Physics.RaycastAll(transform.position, hitAdjustedHeight - lastFramePos, planarRayLength);
			foreach(RaycastHit flatHit in hits)
			{
				if(flatHit.transform.tag == "Arrow")
					flatHit.transform.GetComponent<Arrow>().hit();
			}


			lastFramePos = hitAdjustedHeight;
		}
	}

	public bool LoadNextLevel()
	{
		currentSessionLvl++;
		if(currentSessionLvl >= sessions[currentSession].levels.Count)
		{
			currentSession++;
			currentSessionLvl = 0;
			if(currentSession >= sessions.Length)
			{
				currentSession = 0; //start back at the begining
				return false;
			}
		}

		currentLvlPositions = sessions[currentSession].levels[currentSessionLvl].levelData;
		loadCurrentLevel();
		return true;
	}

	public void loadCurrentLevel()
	{
		arrowManager.setUpArrows(currentLvlPositions, new Vector3(0,1,0));
	}
}
