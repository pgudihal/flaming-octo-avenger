using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelWrapper
{
	public Vector3[] levelData;
	public float levelTime;

	public LevelWrapper(Vector3[] newLevelData, float levelTime)
	{
		levelData = newLevelData;
		this.levelTime = levelTime;
	}
}
