using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour 
{
	public Texture2D[] textures;
	public float shrinkRate;
	public float holdTime = 1;

	private int texIndex = 0;
	private float X = 0;
	private float Y = 0;
	private float fade = 1;

	private float nextSwitchTime;

	//start really matters here since when the game restarts
	//this script is reneabled calling start to reinitialize any vars
	void Start () 
	{
		nextSwitchTime = Time.time + holdTime;
		texIndex = 0;
		X = 0;
		Y = 0;
		fade = 1;
	}

	// Update is called once per frame
	void Update () 
	{
		X += shrinkRate * Time.deltaTime;
		Y += shrinkRate * Time.deltaTime;

		if(Time.time >= nextSwitchTime)
		{
			texIndex++;

			if(texIndex >= textures.Length)
				this.enabled = false;

			if(texIndex == textures.Length-1)
			{
				nextSwitchTime = Time.time + holdTime * .5f;
				gameObject.GetComponent<SwipeLevelManager>().startGame();
			}
			else
				nextSwitchTime = Time.time + holdTime;

			X = 0;
			Y = 0;
		}
	}

	void OnGUI() 
	{
		if(texIndex == textures.Length - 1)
		{
			GUI.color = new Color(1,1,1,fade);
			fade -= .95f*Time.deltaTime;
		}

		GUI.DrawTexture(new Rect(X/2, Y/2, Screen.width-X, Screen.height-Y),textures[texIndex],
		                ScaleMode.StretchToFill);
	}
}
