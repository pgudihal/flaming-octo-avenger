using UnityEngine;
using System.Collections;

public class TestLineRenderer : MonoBehaviour {

	public float speed = 1;
	float intervalTime = .5f;

	float nextTime;
	public LineRenderer line;
	public int count = 10;
	Vector3[] pos;
	// Use this for initialization
	void Start () 
	{
		nextTime = Time.time + intervalTime;
		pos = new Vector3[10];
		reset();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > nextTime && count > 0)
		{
			line.SetVertexCount(count--);
			nextTime = Time.time + intervalTime;
		}

		if(count >= 2)
		{
			Vector3 start = pos[count-1];
			Vector3 end = pos[count-2];
			Vector3 dir = end - start;
			dir.Normalize();
			start += dir*speed*Time.deltaTime;
			Debug.Log(dir*speed*Time.deltaTime);
			line.SetPosition(count-1,start);
		}

		if(count == 0 && Time.time > nextTime)
			reset();
	}

	void reset()
	{
		pos[0] = new Vector3(-10,0,0);
		pos[1] = new Vector3(-5,0,0);
		pos[2] = new Vector3(0,0,0);
		pos[3] = new Vector3(5,0,0);
		pos[4] = new Vector3(10,0,0);
		pos[5] = new Vector3(10,0,3);
		pos[6] = new Vector3(10,0,8);
		pos[7] = new Vector3(10,0,20);
		pos[8] = new Vector3(0,0,20);
		pos[9] = new Vector3(0,0,5);


		count = 10;
		line.SetVertexCount(count);
		for(int i = 0; i < count;i++)
			line.SetPosition(i,pos[i]);

		nextTime = Time.time + intervalTime;
	}
}
