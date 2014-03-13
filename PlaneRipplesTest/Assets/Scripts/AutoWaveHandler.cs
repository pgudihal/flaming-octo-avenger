using UnityEngine;
using System.Collections;

public class AutoWaveHandler : MonoBehaviour 
{
	public int Width = 100;
	public int Length = 100;
	public int SegmentsWidth = 10;
	public int SegmentsLength = 10;

	private int vertexSet = 1;//switching between uvset1 and uvset2, this keeps track
	private Mesh mesh;
	private Vector2[] UVSet1;
	private Vector2[] UVSet2;

	//waves vars
	public float speed = 12f;
	public float damping = 5f;
	public float fallOff = 2.1f;//sensitive
	public float disturbStrength = .5f;
	private float mk1,mk2,mk3;
	private float timer;
	private const float TIME_STEP = 0.03323326f;//MADE TO RUN AT 30FPS
	private float timeTillNextUpdate;

	void Start () 
	{
//		Width = Screen.width/10 + 2;
//		Length = Screen.height/10 + 2;
//		SegmentsWidth = Width;
//		SegmentsLength = Length;

		//creating vertices
		Vector3[] verts = new Vector3[SegmentsWidth*SegmentsLength];
		
		UVSet1 = new Vector2[SegmentsWidth*SegmentsLength];
		UVSet2 = new Vector2[SegmentsWidth*SegmentsLength];
//		
//		float halfway = (length-1)*dx*.5f;
//		for(int i = 0; i < length; ++i)
//		{
//			float z = halfway - i*dx + yCenter-dx;
//			for(int j = 0; j < length; ++j)
//			{
//				float x = -halfway + j*dx + xCenter-dx;
//				verts[i*length+j] = new Vector3(x,0,z);
//				
//				//color values i will pass to the shader via vertex color
//				UVSet1[i*length+j] = Vector2.zero;
//				UVSet2[i*length+j] = Vector2.zero;
//			}
//		}



		float dx = Width/(float)SegmentsWidth;
		float dy = Length/(float) SegmentsLength;
		
		//so that its centered around the origin
		float halfX = (SegmentsWidth-1)*dx*.5f;
		float halfY = (SegmentsLength-1)*dy*.5f;
		
		for(int i = 0; i < SegmentsLength; ++i)
		{
			float y = halfY - i*dy;
			for(int j = 0; j < SegmentsWidth; ++j)
			{
				float x = -halfX + j*dx;
				verts[i*SegmentsWidth+j] = new Vector3(x,0,y);
				
				//color values i will pass to the shader via vertex color
				UVSet1[i*SegmentsWidth+j] = Vector2.zero;
				UVSet2[i*SegmentsWidth+j] = Vector2.zero;
			}
		}
		
		//creating triangles
//		int faces = (length-1)*(length-1)*2;
//		int indices = faces*3;
//		newTriangles = new int[indices];
//		int k = 0;
//		for(int i = 0; i < length-1; ++i)
//		{
//			for(int j = 0; j < length-1; ++j)
//			{
//				newTriangles[k]   = i*length+j;
//				newTriangles[k+1] = i*length+j+1;
//				newTriangles[k+2] = (i+1)*length+j;
//				
//				newTriangles[k+3] = (i+1)*length+j;
//				newTriangles[k+4] = i*length+j+1;
//				newTriangles[k+5] = (i+1)*length+j+1;
//				
//				k += 6; // next quad
//			}
//		}

		int faces = (SegmentsWidth-1)*(SegmentsLength-1)*2;
		int indices = faces*3;
		int []triangles = new int[indices];
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
//		float du = 1.0f/length;
//		float dv = 1.0f/length;
//		newUV = new Vector2[length*length];
//		for(int i = 0; i < length; ++i)
//		{
//			float v = (length - i)*dv;//mapping V correctly
//			for(int j = 0; j < length; ++j)
//			{
//				float u =j*du;
//				newUV[i*length+j] = new Vector2(u,v);
//			}
//		}
//		

		float du = 1.0f/SegmentsWidth;
		float dv = 1.0f/SegmentsLength;
		Vector2[] newUV = new Vector2[SegmentsWidth*SegmentsLength];
		for(int i = 0; i < SegmentsLength; ++i)
		{
			float v = (SegmentsLength - i)*dv;//mapping V correctly
			for(int j = 0; j < SegmentsWidth; ++j)
			{
				float u =j*du;
				newUV[i*SegmentsWidth+j] = new Vector2(u,v);
			}
		}


		//mesh creation
		mesh = new Mesh();
		mesh.name = "WavePlaneMesh";
		mesh.vertices = verts;
		mesh.uv = newUV;
		mesh.triangles = triangles;
		mesh.uv2 = UVSet1;
		
		//mesh.RecalculateNormals();//TODO - Optimize for a plane
		//solveTangents(mesh);//TODO - Optimize for a plane
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		
		//waves vars -- KERNEL calculation
		float d = damping*TIME_STEP+fallOff;
		float e = (speed*speed)*(TIME_STEP*TIME_STEP)/(dx*dx);
		mk1 = (damping*TIME_STEP-2.0f)/ d;
		mk2 = (4.0f-8.0f*e) / d;
		mk3 = (2.0f*e) / d;
		
		
		//adding a mesh collider
		gameObject.AddComponent("MeshCollider");
	}
	
	void Update () 
	{	
		if(Input.GetKeyDown(KeyCode.Space))disturbRandom();
		if(Input.GetMouseButton(0)) hit();
		
		if(vertexSet == 1)
		{
			vertexSet++;//next time use vertex set 2
			for(int i = 1; i < SegmentsLength-1; ++i)
			{
				for(int j = 1; j < SegmentsWidth-1; ++j)
				{
					UVSet2[i*SegmentsWidth+j].x = 
						mk1*UVSet2[i*SegmentsWidth+j].x +
							mk2*UVSet1[i*SegmentsWidth+j].x +
							mk3*(UVSet1[(i+1)*SegmentsWidth+j].x + 
							     UVSet1[(i-1)*SegmentsWidth+j].x+ 
							     UVSet1[i*SegmentsWidth+j+1].x+  
							     UVSet1[i*SegmentsWidth+j-1].x);
				}
			}
			mesh.uv2 = UVSet2;
		}
		else
		{
			vertexSet--;
			for(int i = 1; i < SegmentsLength-1; ++i)
			{
				for(int j = 1; j < SegmentsWidth-1; ++j)
				{
					UVSet1[i*SegmentsWidth+j].x = 
						mk1*UVSet1[i*SegmentsWidth+j].x  +
							mk2*UVSet2[i*SegmentsWidth+j].x +
							mk3*(UVSet2[(i+1)*SegmentsWidth+j].x + 
							     UVSet2[(i-1)*SegmentsWidth+j].x + 
							     UVSet2[i*SegmentsWidth+j+1].x + 
							     UVSet2[i*SegmentsWidth+j-1].x);
				}
			}
			
			mesh.uv2 = UVSet1;
		}
	}
	
	public void disturbRandom()
	{
		int i = Random.Range(2,SegmentsLength - 2);
		int j = Random.Range(2,SegmentsWidth - 2);
		
		//float half = .5f * disturbStrength;
		
		
		if(vertexSet == 1)
		{
			UVSet1[i*SegmentsWidth+j].x += disturbStrength;
			UVSet1[i*SegmentsWidth+j+1].x += disturbStrength;
			UVSet1[i*SegmentsWidth+j-1].x += disturbStrength;
			UVSet1[(i+1)*SegmentsWidth+j].x += disturbStrength;
			UVSet1[(i-1)*SegmentsWidth+j].x += disturbStrength;
		}
		else
		{
			UVSet2[i*SegmentsWidth+j].x += disturbStrength;
			UVSet2[i*SegmentsWidth+j+1].x += disturbStrength;
			UVSet2[i*SegmentsWidth+j-1].x += disturbStrength;
			UVSet2[(i+1)*SegmentsWidth+j].x += disturbStrength;
			UVSet2[(i-1)*SegmentsWidth+j].x += disturbStrength;
		}
	}
	
	//automatically finds the closest vertex when given a world point
	public void disturbPoint(Vector3 point, int triangleIndex)
	{
		Vector2 vertex = NearestVertexTo(point, triangleIndex);

		int i = Mathf.Clamp((int)vertex.x,3,SegmentsLength-3);
		int j = Mathf.Clamp((int)vertex.y,3,SegmentsWidth-3);

		if(vertexSet == 1)
		{
			UVSet1[i*SegmentsWidth+j].x += disturbStrength;
			UVSet1[i*SegmentsWidth+j+1].x += disturbStrength;
			UVSet1[i*SegmentsWidth+j-1].x += disturbStrength;
			UVSet1[(i+1)*SegmentsWidth+j].x += disturbStrength;
			UVSet1[(i-1)*SegmentsWidth+j].x += disturbStrength;
		}
		else
		{
			UVSet2[i*SegmentsWidth+j].x += disturbStrength;
			UVSet2[i*SegmentsWidth+j+1].x += disturbStrength;
			UVSet2[i*SegmentsWidth+j-1].x += disturbStrength;
			UVSet2[(i+1)*SegmentsWidth+j].x += disturbStrength;
			UVSet2[(i-1)*SegmentsWidth+j].x += disturbStrength;
		}
	}

	private void hit()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (collider.Raycast (ray, out hit, 1000.0f)) 
			disturbPoint(hit.point,hit.triangleIndex);
	}
	
	//finds the nearest index of the vertex closest to the point that was disturbed
	//triangleIndex is a hint
	private Vector2 NearestVertexTo(Vector3 point, int triangleIndex)
	{
		// convert point to local space
		point = transform.InverseTransformPoint(point);
		
		float minDistanceSqr = Mathf.Infinity;
		Vector3 vertex = Vector3.zero;
		int index = 0;
		
		// scan only the vertices of the triangle hit
		for (int i = 0; i < 3; i++)
		{
			vertex = mesh.vertices[mesh.triangles[triangleIndex*3 + i]];
			
			Vector3 diff = point-vertex;
			float distSqr = diff.sqrMagnitude;
			
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				index = mesh.triangles[triangleIndex*3 + i];
			}
		}

		return new Vector2(index/SegmentsWidth,index%SegmentsWidth);
	}
	
//	private void solveTangents(Mesh mesh)
//	{
//		//speed up math by copying the mesh arrays
//		int[] triangles = mesh.triangles;
//		Vector3[] vertices = mesh.vertices;
//		Vector2[] uv = mesh.uv;
//		Vector3[] normals = mesh.normals;
//		
//		//variable definitions
//		int triangleCount = triangles.Length;
//		int vertexCount = vertices.Length;
//		
//		Vector3[] tan1 = new Vector3[vertexCount];
//		Vector3[] tan2 = new Vector3[vertexCount];
//		
//		Vector4[] tangents = new Vector4[vertexCount];
//		
//		for (long a = 0; a < triangleCount; a += 3)
//		{
//			long i1 = triangles[a + 0];
//			long i2 = triangles[a + 1];
//			long i3 = triangles[a + 2];
//			
//			Vector3 v1 = vertices[i1];
//			Vector3 v2 = vertices[i2];
//			Vector3 v3 = vertices[i3];
//			
//			Vector2 w1 = uv[i1];
//			Vector2 w2 = uv[i2];
//			Vector2 w3 = uv[i3];
//			
//			float x1 = v2.x - v1.x;
//			float x2 = v3.x - v1.x;
//			float y1 = v2.y - v1.y;
//			float y2 = v3.y - v1.y;
//			float z1 = v2.z - v1.z;
//			float z2 = v3.z - v1.z;
//			
//			float s1 = w2.x - w1.x;
//			float s2 = w3.x - w1.x;
//			float t1 = w2.y - w1.y;
//			float t2 = w3.y - w1.y;
//			
//			float r = 1.0f / (s1 * t2 - s2 * t1);
//			
//			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
//			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
//			
//			tan1[i1] += sdir;
//			tan1[i2] += sdir;
//			tan1[i3] += sdir;
//			
//			tan2[i1] += tdir;
//			tan2[i2] += tdir;
//			tan2[i3] += tdir;
//		}
//		
//		
//		for (long a = 0; a < vertexCount; ++a)
//		{
//			Vector3 n = normals[a];
//			Vector3 t = tan1[a];
//			
//			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
//			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
//			Vector3.OrthoNormalize(ref n, ref t);
//			tangents[a].x = t.x;
//			tangents[a].y = t.y;
//			tangents[a].z = t.z;
//			
//			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
//		}
//		
//		mesh.tangents = tangents;
//	}
}
