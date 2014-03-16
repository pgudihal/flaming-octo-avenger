using UnityEngine;
using System.Collections;

public class SwipeLevelManager : MonoBehaviour 
{

	public SwipeSession[] sessions;
	public float HitHeight = 1;
	public GUIStyle timerStyle;
	public GUIStyle gameOverStyle;
	public UILabel timerUI, gameOver, finalScore;

	public LayerMask SwipeHitOnlyMask;
	private Vector3[] currentLvlPositions;
	private int currentSession = 0;
	private int currentSessionLvl = 0;
	private Vector3 lastFramePos = new Vector3(0,1,0);
	public ArrowManager arrowManager;
	private bool hasGameStarted = false;
	private ScoreKeeper scoreKeeper;
	private float timeLeft;
	private bool isGameOver = false;//isgameover is ONLY really used for the game over GUI to come up
	private int currentRepeat = 0;

	// Use this for initialization
	void Start () 
	{
		scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
		currentLvlPositions = sessions[currentSession].levels[currentSessionLvl].levelData;
		timerUI = GameObject.Find ("Timer").GetComponent<UILabel> ();
		gameOver = GameObject.Find ("Game Over").GetComponent<UILabel> ();
		finalScore = GameObject.Find ("Final Score").GetComponent<UILabel> ();
		loadCurrentLevel();
	}

	public void startGame()
	{
		hasGameStarted = true;
		isGameOver = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isGameOver) {
			timerUI.text = timeLeft.ToString();
			gameOver.enabled = false;
			finalScore.enabled = false;
		}
		else 
		{
			timerUI.text = "";
			gameOver.enabled = true;
			finalScore.enabled = true;
			gameOver.text = "GAME OVER";
			finalScore.text = "Final Score: " + scoreKeeper.TotalScore;
		}			
		if(!hasGameStarted || isGameOver) return;
		if(arrowManager.areArrowsReturned()) LoadNextLevel();
		if(Input.GetMouseButton(0)) pressing();

		if(timeLeft <= 0)
			isGameOver = true;
		else
			timeLeft -= Time.deltaTime;
	}

	/*void OnGUI()
	{
		if(isGameOver)
		{
			//GUI.Box(new Rect(Screen.width/4,Screen.height/4,Screen.width/2,Screen.height/2),"Game Over",gameOverStyle);
			//timerStyle.fontSize = 30;
			//GUI.Box(new Rect(Screen.width/4 + 200,Screen.height/4 + 200,Screen.width/2,Screen.height/2),"Score: " + scoreKeeper.TotalScore,timerStyle);
		}
		else
		{
			//GUI.Box(new Rect(Screen.width - 200,10,200,20),"Time: " + timeLeft,timerStyle);
		}
	}*/

	private void pressing()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000))
		{
			//removing the hit arrow from the initial raycast
			Debug.DrawLine(ray.origin, hit.point);
			if(hit.transform.tag == "Arrow" && hit.transform.GetComponent<Arrow>().hit())
			{
				scoreKeeper.addScore();

				if(hit.transform.GetComponent<Arrow>().IsEndingArrow)
					scoreKeeper.giveTime(.4f);
			}

			//in order to account for fast swipes we can raycast from the last frame's hit
			//point to the current frames hit point. this raycast is parralel to the 
			//plane
			Vector3 hitAdjustedHeight = new Vector3(hit.point.x,1,hit.point.z);

			RaycastHit[] hits;
			hits = Physics.RaycastAll(lastFramePos, hitAdjustedHeight - lastFramePos, Vector3.Distance(lastFramePos,hitAdjustedHeight), SwipeHitOnlyMask);
			foreach(RaycastHit flatHit in hits)
			{
				if(flatHit.transform.GetComponent<Arrow>().hit())
				{
					scoreKeeper.addScore();

					if(flatHit.transform.GetComponent<Arrow>().IsEndingArrow)
						scoreKeeper.giveTime(.4f);
				}
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
				currentRepeat++;
				return false;
			}
		}

		currentLvlPositions = sessions[currentSession].levels[currentSessionLvl].levelData;
		loadCurrentLevel();
		return true;
	}

	public int getCurrentLevel()
	{
		return (currentRepeat + 1) * (currentSession + 1) * (currentSessionLvl + 1);
	}

	public void loadCurrentLevel()
	{
		timeLeft += sessions[currentSession].levels[currentSessionLvl].levelTime;
		arrowManager.setUpArrows(currentLvlPositions, new Vector3(0,1,0));
	}
}
