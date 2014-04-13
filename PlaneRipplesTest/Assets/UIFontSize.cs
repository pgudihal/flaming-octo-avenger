using UnityEngine;
using System.Collections;

public class UIFontSize : MonoBehaviour {
	Resolution curRes;
	public UIFont radioStars;
	public UILabel score, timer;
	// Use this for initialization
	void Start () {
		curRes = Screen.currentResolution;
		if (curRes.height < 800 && curRes.width < 1280){
			score.fontSize = 40;
			timer.fontSize = 40;
		}
		else{
			score.fontSize = 53;
			timer.fontSize = 40;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
