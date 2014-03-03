using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SwipeSession : ScriptableObject {

	public string name = "";
	public List<Vector3[]> levels = new List<Vector3[]>();
}
