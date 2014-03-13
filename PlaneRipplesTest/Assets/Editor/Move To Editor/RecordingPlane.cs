using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RecordingPlane : MonoBehaviour 
{
	public int Width = 100;
	public int Length = 100;
	public int SegmentsWidth = 10;
	public int SegmentsLength = 10;


	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;
	private Color32[] vColors;

	private Color32 WHITE = new Color32(255,255,255,255);
	private Color32 BLACK = new Color32(0,0,0,1);

	//info is that is kept in tempQ can be undone, it keeps triangle indeces
	//Also this is stupid and I dont need two stacks but i dont feel like redoing it
	private Stack<int> tempQ = new Stack<int>();
	private Stack<int> record = new Stack<int>();
	private SwipeSession session;
	private int lastPush;//counts how many points were added to the record last mouse down
	public static Vector3 END_SWIPE = new Vector3(0,-100,100);//put inbetween swipes when saving
	public static int END_SWIPE_INDEX = -1; //put inbetween swipes when pushing on to record

	
	void Start () 
	{
		float dx = Width/(float)SegmentsWidth;
		float dy = Length/(float) SegmentsLength;

		//so that its centered around the origin
		float halfX = (SegmentsWidth-1)*dx*.5f;
		float halfY = (SegmentsLength-1)*dy*.5f;

		vertices = new Vector3[SegmentsWidth * SegmentsLength];
		vColors = new Color32[SegmentsWidth * SegmentsLength];

		for(int i = 0; i < SegmentsLength; ++i)
		{
			float y = halfY - i*dy;
			for(int j = 0; j < SegmentsWidth; ++j)
			{
				float x = -halfX + j*dx;
				vertices[i*SegmentsWidth+j] = new Vector3(x,0,y);
				vColors[i*SegmentsWidth+j] = WHITE;
			}
		}

		//creating triangles
		int faces = (SegmentsWidth-1)*(SegmentsLength-1)*2;
		int indices = faces*3;
		triangles = new int[indices];
		int k = 0;
		for(int i = 0; i < SegmentsLength-1; ++i)
		{
			for(int j = 0; j < SegmentsWidth-1; ++j)
			{
				triangles[k]   = i*SegmentsWidth+j;
				triangles[k+1] = i*SegmentsWidth+j+1;
				triangles[k+2] = (i+1)*SegmentsWidth+j;
				
				triangles[k+3] = (i+1)*SegmentsWidth+j;
				triangles[k+4] = i*SegmentsWidth+j+1;
				triangles[k+5] = (i+1)*SegmentsWidth+j+1;
				
				k += 6; // next quad
			}
		}
		
		//maping UVs
		float du = 1.0f/SegmentsWidth;
		float dv = 1.0f/SegmentsLength;
		Vector2[] UVs = new Vector2[SegmentsWidth * SegmentsLength];
		Vector2[] UV1 = new Vector2[SegmentsWidth * SegmentsLength];
		for(int i = 0; i < SegmentsLength; ++i)
		{
			float v = (SegmentsLength - i)*dv;//mapping V correctly
			for(int j = 0; j < SegmentsWidth; ++j)
			{
				float u =j*du;
				UVs[i*SegmentsWidth+j] = new Vector2(u,v);
				UV1[i*SegmentsWidth+j] = new Vector2(j%2,i%2);//barycentric UVs for shader
			}
		}
		
		//mesh creation
		mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.colors32 = vColors;
		mesh.uv = UVs;
		mesh.uv1 = UV1;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent("MeshCollider");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!hasSessionStarted) return;
		if(Input.GetMouseButton(0)) readHitPosition();
		if(Input.GetMouseButtonUp(0)) recordHitPosition();
		if(Input.GetKeyDown(KeyCode.Space)) undo();
	}

	string tempString = "Enter Session Name";
	string lvlTime = "3";
	bool hasSessionStarted = false;
	void OnGUI()
	{
		if(!hasSessionStarted)
		{
			tempString = GUI.TextField(new Rect(10, 5, 200, 20), tempString, 25);

			if (GUI.Button(new Rect(220, 5, 50, 20), "Enter"))
			{
				if(tempString != "" && tempString != "Enter Session Name") 
				{
					hasSessionStarted = true;
					session = ScriptableObject.CreateInstance<SwipeSession>();
					session.sessionName = tempString;
				}
			}
			return;
		}

		lvlTime = GUI.TextField(new Rect(Screen.width-50,5,40,20),lvlTime,10);

		if (GUI.Button(new Rect(Screen.width-190, 5, 120, 20), "Save Level"))
			saveLevel();

		if (GUI.Button(new Rect(Screen.width-190, Screen.height-25, 180, 20), "Save and Write Session"))
		{
			saveAndWriteSession();
			hasSessionStarted = false;
			tempString = "Enter Session Name";
		}

		if(GUI.Button(new Rect(10,5,50,20),"Clear"))
			clear();
	}

	private void readHitPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (collider.Raycast (ray, out hit, 1000.0f)) 
		{
			Debug.DrawLine (ray.origin, hit.point);
			vColors[triangles[hit.triangleIndex*3    ]] = BLACK;
			vColors[triangles[hit.triangleIndex*3 + 1]] = BLACK;
			vColors[triangles[hit.triangleIndex*3 + 2]] = BLACK;

			if(tempQ.Count == 0 || hit.triangleIndex != tempQ.Peek())
				tempQ.Push(hit.triangleIndex);
		}
		mesh.colors32 = vColors;
	}

	private void recordHitPosition()
	{
		lastPush = tempQ.Count;
		record.Push(END_SWIPE_INDEX);
		while(tempQ.Count > 0)
			record.Push(tempQ.Pop());
	}
	
	private Vector3 findTriangleCenter(int triangleIndex)
	{
		if(triangleIndex < 0)
			return RecordingPlane.END_SWIPE;

		return vectorMidpoint(
			vectorMidpoint(vertices[triangles[triangleIndex*3    ]], vertices[triangles[triangleIndex*3 + 1]]),
			vectorMidpoint(vertices[triangles[triangleIndex*3    ]], vertices[triangles[triangleIndex*3 + 2]]));
	}

	private void undo()
	{
		while(lastPush > 0)
		{
			int triangleIndex = record.Pop();
			vColors[triangles[triangleIndex*3    ]] = WHITE;
			vColors[triangles[triangleIndex*3 + 1]] = WHITE;
			vColors[triangles[triangleIndex*3 + 2]] = WHITE;

			lastPush--;
		}

		record.Pop();//getting rid of END_SWIPE_INDEX

		mesh.colors32 = vColors;
	}

	private void clear()
	{
		for(int i = 0; i < vColors.Length;i++)
			vColors[i] = WHITE;
		mesh.colors32 = vColors;

		tempQ.Clear();
		record.Clear();
		lastPush = 0;
	}

	private void saveAndWriteSession()
	{
		saveLevel();

		CustomAssetUtility.CreateAssetAtPath("SwipeSessions",session.sessionName,session);
	}

	private void saveLevel()
	{

		Vector3[] lvlData = new Vector3[record.Count];

		for(int i = 0; i < lvlData.Length;i++)
			lvlData[i] = findTriangleCenter(record.Pop());

		if(lvlTime == "")
			lvlTime = "3";

		session.levels.Add(new LevelWrapper(lvlData,float.Parse(lvlTime)));
		lvlTime = "3";
		clear();
	}

	private Vector3 vectorMidpoint(Vector3 a,Vector3 b)
	{
		return (a + b)/2;
	}

}
