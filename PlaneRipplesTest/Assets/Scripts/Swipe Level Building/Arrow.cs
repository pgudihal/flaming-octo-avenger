using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour 
{
	public bool IsOnBoard {get{return isOnBoard;}}
	public bool IsEndingArrow{get{return isUsingEndMat;}}
	public Material ArrowMat;
	public Material EndMat;

	private ArrowManager manager;

	private bool isOnBoard = false;//is it currently waiting to be hit
	private Arrow previousArrow;
	private Transform nextArrow;

	//used so that on the first update this arrow will look at its next arrow
	private bool isLookingAt = false;
	private bool isUsingEndMat = false;
	private SoundFXManager soundFX;

	private Arrow(bool isOnBoard)
	{
		this.isOnBoard = isOnBoard;
	}
	// Use this for initialization
	void Start () 
	{
		manager = transform.parent.gameObject.GetComponent<ArrowManager>();
		soundFX = GameObject.Find("SoundMistro").GetComponent<SoundFXManager>();
		renderer.enabled = false;
		collider.enabled = false;
	}

	public void reset()
	{
		isOnBoard = false;
		isLookingAt = false;
		renderer.enabled = false;
		collider.enabled = false;
	}

	void Update()
	{
		if(!isLookingAt)
	   	{
			isLookingAt = true;
			if(nextArrow != null)
				transform.LookAt(nextArrow);
		}
	}

	public void initialize(Transform nextArrow)
	{
		this.nextArrow = nextArrow;
	}

	public void setAsEndArrow()
	{
		isUsingEndMat = true;
		renderer.material = EndMat;
	}

	//previous is to tell this arrow which came before it.
	//this arrow can only be hit if the one before it was hit prior
	public void setup(Vector3 pos, Arrow previousArrow)
	{
		transform.position = pos;
		renderer.enabled = true;
		collider.enabled = true;
		isOnBoard = true;
		this.previousArrow = previousArrow;

		isLookingAt = false;
		if(isUsingEndMat)
		{
			renderer.material = ArrowMat;
			isUsingEndMat = false;
		}
	}

	public void setup(Vector3 pos)
	{
		setup(pos,null);
	}

	//return true if its a valid hit
	public bool hit()
	{
		if(!isOnBoard || (previousArrow != null && previousArrow.IsOnBoard)) return false;

		manager.arrowReturned();
		renderer.enabled = false;
		collider.enabled = false;
		isOnBoard = false;

		if(IsEndingArrow)
		{
			soundFX.playFX(SoundFXManager.FXNames.FinishedSwipe);
		}

		return true;
	}
}
