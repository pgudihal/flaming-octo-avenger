using UnityEngine;
using System.Collections;

public class SwipeLevelManager : MonoBehaviour 
{

	public SwipeSession[] sessions;

	private Vector3[] currentLvlPositions;
	private int currentSession = 0;
	private int currentSessionLvl = 0;
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
			Debug.DrawLine(ray.origin, hit.point);
			if(hit.transform.tag != "Arrow") return;

			hit.transform.GetComponent<Arrow>().hit();
		}
	}

	public bool LoadNextLevel()
	{
		currentSessionLvl++;
		if(currentSessionLvl > currentLvlPositions.Length)
		{
			currentSession++;
			currentSessionLvl = 0;
			if(currentSession > sessions.Length)
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
