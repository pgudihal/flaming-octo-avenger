using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour 
{
	public bool IsOnBoard {get{return isOnBoard;}}

	private ArrowManager manager;

	private bool isOnBoard = false;//is it currently waiting to be hit
	private Arrow previousArrow;
	private Transform nextArrow;

	private Arrow(bool isOnBoard)
	{
		this.isOnBoard = isOnBoard;
	}
	// Use this for initialization
	void Start () 
	{
		manager = transform.parent.gameObject.GetComponent<ArrowManager>();
		renderer.enabled = false;
		collider.enabled = false;
		nextArrow = null;
	}

	public void initialize(Transform nextArrow)
	{
		this.nextArrow = nextArrow;
	}

	//previous is to tell this arrow which came before it.
	//this arrow can only be hit if the one before it was hit prior
	public void setup(Vector3 pos, Arrow previousArrow)
	{
		transform.position = pos;
		renderer.enabled = true;
		collider.enabled = true;
		this.previousArrow = previousArrow;

		if(nextArrow != null)
			transform.LookAt(nextArrow);
	}

	public void hit()
	{
		if(previousArrow.IsOnBoard) return;

		manager.arrowReturned();
		renderer.enabled = false;
		renderer.enabled = false;
		isOnBoard = false;
	}

	public void setSpecial()
	{
		isOnBoard = false;
	}

	public static Arrow InitialPreviousArrow()
	{
		return new Arrow(false);
	}
}
