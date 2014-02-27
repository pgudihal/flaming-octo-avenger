using UnityEngine;
using System.Collections;

public class RecordingPlane : MonoBehaviour 
{
	public int Width = 100;
	public int Length = 100;
	public int SegmentsWidth = 10;
	public int SegmentsLength = 10;


	private Mesh mesh;
	// Use this for initialization
	void Start () 
	{
		float dx = Width/(float)SegmentsWidth;
		float dy = Length/(float) SegmentsLength;

		//so that its centered around the origin
		float halfX = (SegmentsWidth-1)*dx*.5f;
		float halfY = (SegmentsLength-1)*dy*.5f;

		Vector3[] vertices = new Vector3[SegmentsWidth * SegmentsLength];

		for(int i = 0; i < SegmentsLength; ++i)
		{
			float y = halfY - i*dy;
			for(int j = 0; j < SegmentsWidth; ++j)
			{
				float x = -halfX + j*dx;
				vertices[i*SegmentsWidth+j] = new Vector3(x,0,y);
			}
		}

		//creating triangles
		int faces = (SegmentsWidth-1)*(SegmentsLength-1)*2;
		int indices = faces*3;
		int[] triangles = new int[indices];
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
		for(int i = 0; i < SegmentsLength; ++i)
		{
			float v = (SegmentsLength - i)*dv;//mapping V correctly
			for(int j = 0; j < SegmentsWidth; ++j)
			{
				float u =j*du;
				UVs[i*SegmentsWidth+j] = new Vector2(u,v);
			}
		}
		
		//mesh creation
		mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = UVs;
		mesh.triangles = triangles;
		GetComponent<MeshFilter>().mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
