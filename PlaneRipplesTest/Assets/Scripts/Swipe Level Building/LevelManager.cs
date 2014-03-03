using UnityEngine;
using System.Collections;

public class SwipeManager : MonoBehaviour 
{

	public string[] sessionNames;

	private int currentSession;
	private int LvlsInSession;
	private int currentLvl;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool LoadNextLevel()
	{
		return false;
	}

	public bool PrepNextSession()
	{
		return false;
	}
}
