using UnityEngine;
using System.Collections;

public class SceneDefaults : MonoBehaviour {

	public int FrameRate = 32;

	void Awake()
	{
		Application.targetFrameRate = 30;
		QualitySettings.vSyncCount = 2;
	}

}
