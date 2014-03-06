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

	private float nextSwitchTime;
	void Start () 
	{
		nextSwitchTime = Time.time + holdTime;
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
			{
				gameObject.GetComponent<SwipeLevelManager>().startGame();
				this.enabled = false;
			}
			X = 0;
			Y = 0;
			nextSwitchTime = Time.time + holdTime;
		}
	}

	void OnGUI() 
	{
		GUI.DrawTexture(new Rect(X/2, Y/2, Screen.width-X, Screen.height-Y),textures[texIndex],
		                ScaleMode.StretchToFill);
	}
}
