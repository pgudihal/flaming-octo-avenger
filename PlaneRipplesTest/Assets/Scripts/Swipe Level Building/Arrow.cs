﻿using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour 
{
	public bool IsOnBoard {get{return isOnBoard;}}

	private ArrowManager manager;

	private bool isOnBoard = false;//is it currently waiting to be hit
	private Arrow previousArrow;
	private Transform nextArrow;

	private bool isLookingAt = false;

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
	}

	public void setup(Vector3 pos)
	{
		setup(pos,null);
	}

	public void hit()
	{
		if(previousArrow != null && previousArrow.IsOnBoard) return;

		manager.arrowReturned();
		renderer.enabled = false;
		collider.enabled = false;
		isOnBoard = false;
	}
}
