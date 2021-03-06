﻿using UnityEngine;
using System.Collections;

public class ArrowManager : MonoBehaviour 
{
	public int arrowCount = 200;
	public GameObject arrowModel;
	public Arrow[] arrows;
	public float minDistance = 1.0f;

	public int ActiveArrowCount { get{ return activeArrowCount;}}

	private int activeArrowCount = 0;

	void Awake()
	{
		arrows = new Arrow[arrowCount];
		for(int i = 0; i < arrowCount; i++)
		{
			GameObject tempArrow = Instantiate(arrowModel) as GameObject;
			tempArrow.transform.parent = transform;
			arrows[i] = tempArrow.GetComponent<Arrow>();
		}

		//initializing arrows
		for(int i = 0; i < arrowCount-1;i++)
			arrows[i].initialize(arrows[i+1].transform);

		arrows[arrowCount-1].initialize(null);
	}

	public void reset()
	{
		for(int i = 0; i < arrowCount; i++)
			arrows[i].reset();
		activeArrowCount = 0;
	}

	public void arrowReturned()
	{
		activeArrowCount--;
	}

	public bool areArrowsReturned()
	{
		return activeArrowCount == 0;
	}
	public static Vector3 END_SWIPE = new Vector3(0,-100,100);
	public bool setUpArrows(Vector3[] positions, Vector3 heightOffset)
	{
		activeArrowCount = 0;
		int i = 0;
		//incase the first few indexers point to endswipes
		while(positions[i] == END_SWIPE)
			i++;

		arrows[activeArrowCount].setup(positions[i] + heightOffset);
		Arrow previous = arrows[activeArrowCount];
		i++;
		activeArrowCount++;

		for(; i < positions.Length; i++)
		{
			if(positions[i] == END_SWIPE)
			{
				if(previous != null) previous.setAsEndArrow();
				previous = null;
			}
			else
			{
				//testing to make sure arrows are spaced evenly
				if(Vector3.Distance(arrows[activeArrowCount-1].transform.position,
				                    positions[i] + heightOffset) > minDistance)
				{
					arrows[activeArrowCount].setup(positions[i] + heightOffset,previous);
					previous = arrows[activeArrowCount];
					activeArrowCount++;

					if(activeArrowCount >= positions.Length)
					{
						Debug.Log("Not Enough Arrows");
						return false;
					}
				}
			}
		}
		return true;
	}
}
