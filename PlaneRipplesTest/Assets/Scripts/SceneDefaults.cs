using UnityEngine;
using System.Collections;

public class SceneDefaults : MonoBehaviour {

	public int FrameRate = 32;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}

}
