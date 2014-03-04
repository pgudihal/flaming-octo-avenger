using UnityEngine;
using System.Collections;

public class ArrowManager : MonoBehaviour 
{
	public int arrowCount = 200;
	public GameObject arrowModel;
	public Arrow[] arrows;

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
	}
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void arrowReturned()
	{
		activeArrowCount++;
	}

	public bool areArrowsReturned()
	{
		return activeArrowCount == 0;
	}

	public bool setUpArrows(Vector3[] positions, Vector3 heightOffset)
	{
		if(positions.Length > arrowCount)
		{
			Debug.Log("Not Enough Arrows");
			return false;
		}

		Arrow initPrev = Arrow.InitialPreviousArrow();
		Arrow previous = initPrev;
		arrows[0].setup(positions[0] + heightOffset,previous);
		previous = arrows[0];
		activeArrowCount++;

		for(int i = 1; i < positions.Length; i++)
		{
			activeArrowCount++;
			arrows[i].setup(positions[i] + heightOffset,previous);
			previous = arrows[i];
		}

		return true;
	}
}
