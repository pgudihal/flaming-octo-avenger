using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelWrapper
{
	public Vector3[] levelData;

	public LevelWrapper(Vector3[] newLevelData)
	{
		levelData = newLevelData;
	}
}
