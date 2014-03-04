using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SwipeSession : ScriptableObject {

	public string sessionName = "";
	public List<LevelWrapper> levels = new List<LevelWrapper>();
}
