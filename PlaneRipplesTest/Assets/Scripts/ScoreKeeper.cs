using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour 
{
	public int TotalScore {get{return totalScore;}}

	public int arrowScore = 1;
	//how long they have to hit the next arrow so that it doesnt break their score
	public float minDownTime = .4f;
	public GUIStyle style;



	private int totalScore = 0;
	private int currentRollScore = 0;
	private int currentRollCount = 0;
	private int oldMultiplier = 5;
	private float lastScoreTime = 0;

	private GUIText guitext;
	private const int FONT_SIZE = 10;
	public int addFontAmount = 5;
	private bool isOnRoll = false;//so that the sound only plays once
	private SoundFXManager soundFX;

	void Start () 
	{
		guitext = gameObject.GetComponent<GUIText>();
		soundFX = GameObject.Find("SoundMistro").GetComponent<SoundFXManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > (lastScoreTime + minDownTime) && isOnRoll)
			breakRoll();

		if(Input.GetMouseButton(0)) showRollScore();
		if(Input.GetMouseButtonUp(0)) guitext.text = "";
	}

	private void showRollScore()
	{
		if(currentRollCount > 0)
		{
			guitext.text = "" + currentRollScore;
			Vector3 newPos = new Vector3((Input.mousePosition.x - guitext.fontSize)/Screen.width,
			                             (Input.mousePosition.y + guitext.fontSize)/Screen.height,0);
			guitext.transform.position = newPos;
		}
		else
			guitext.text = "";
	}

	public void addScore()
	{

		int rollMultiplier = Mathf.Max(1,(currentRollCount / 5) * 5);
		if(rollMultiplier >= oldMultiplier) 
		{
			oldMultiplier = rollMultiplier;
			growFont();
		}

		currentRollCount++;
		currentRollScore += arrowScore * rollMultiplier;
		totalScore += arrowScore * rollMultiplier;
		lastScoreTime = Time.time;
	}

	private void growFont()
	{
		guitext.fontSize += addFontAmount;
		isOnRoll = true;
	}

	public void giveTime(float t)
	{
		lastScoreTime += t;
	}

	//this function is called when you want to reset the bonuses given
	//when the player is "on a roll"
	public void breakRoll()
	{
		currentRollScore = 0;
		currentRollCount = 0;
		guitext.fontSize = FONT_SIZE;
		soundFX.playFX(SoundFXManager.FXNames.BreakRoll);
		Handheld.Vibrate();
		isOnRoll = false;
	}

	void OnGUI()
	{
		GUI.Box(new Rect(10,10,200,20),"Score: " + TotalScore,style);
	}
}
