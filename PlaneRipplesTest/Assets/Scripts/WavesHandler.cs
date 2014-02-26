using UnityEngine;
using System.Collections;

public class WavesHandler : MonoBehaviour 
{
	private Color[] colorSet1;
	private Color[] colorSet2;
    private Vector2[] newUV;
    private int[] newTriangles;
	
	private float dx = 1f;
	private int length = 100;
	
	public int xCenter = 0;
	public int yCenter = 0;
	
	private int vertexSet = 1;//switching between newVertices1 and newVertices2, this keeps track
	
	//waves vars
	public float speed = 1f;
	public float damping = .4f;
	public float disturbStrength = 1f;
	private float mk1,mk2,mk3;
	private float timer;
	private const float TIME_STEP = 0.03323326f;//MADE TO RUN AT 30FPS
	private float timeTillNextUpdate;
	
	private Mesh mesh;
	 
	void Start () 
	{
		//creating vertices
		Vector3[] verts = new Vector3[length*length];

		colorSet1 = new Color[length*length];
		colorSet2 = new Color[length*length];

		float halfway = (length-1)*dx*.5f;
		for(int i = 0; i < length; ++i)
		{
			float z = halfway - i*dx + yCenter-dx;
			for(int j = 0; j < length; ++j)
			{
				float x = -halfway + j*dx + xCenter-dx;
				verts[i*length+j] = new Vector3(x,0,z);

				//color values i will pass to the shader via vertex color
				colorSet1[i*length+j] = new Color(0f,0,0,1);
				colorSet2[i*length+j] = new Color(0f,0,0,1);
			}
		}
		
		//creating triangles
		int faces = (length-1)*(length-1)*2;
		int indices = faces*3;
		newTriangles = new int[indices];
		int k = 0;
		for(int i = 0; i < length-1; ++i)
		{
			for(int j = 0; j < length-1; ++j)
			{
				newTriangles[k]   = i*length+j;
				newTriangles[k+1] = i*length+j+1;
				newTriangles[k+2] = (i+1)*length+j;
	
				newTriangles[k+3] = (i+1)*length+j;
				newTriangles[k+4] = i*length+j+1;
				newTriangles[k+5] = (i+1)*length+j+1;
	
				k += 6; // next quad
			}
		}
		
		//maping UVs
		float du = 1.0f/length;
		float dv = 1.0f/length;
		newUV = new Vector2[length*length];
		for(int i = 0; i < length; ++i)
		{
			float v = (length - i)*dv;//mapping V correctly
			for(int j = 0; j < length; ++j)
			{
				float u =j*du;
				newUV[i*length+j] = new Vector2(u,v);
			}
		}
		
		//mesh creation
		mesh = new Mesh();
		mesh.name = "WavePlaneMesh";
        mesh.vertices = verts;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
		mesh.colors = colorSet1;
		
		mesh.RecalculateNormals();//TODO - Optimize for a plane
		solveTangents(mesh);//TODO - Optimize for a plane
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		
		//waves vars -- KERNEL calculation
		float d = damping*TIME_STEP+2.0f;
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

		if(vertexSet == 1)
		{
			vertexSet++;//next time use vertex set 2
			for(int i = 1; i < length-1; ++i)
			{
				for(int j = 1; j < length-1; ++j)
				{
					colorSet2[i*length+j].r = 
						mk1*colorSet2[i*length+j].r +
						mk2*colorSet1[i*length+j].r +
					  	mk3*(colorSet1[(i+1)*length+j].r + 
					     	 colorSet1[(i-1)*length+j].r+ 
						     colorSet1[i*length+j+1].r+  
					     	 colorSet1[i*length+j-1].r);
				}
			}
			mesh.colors = colorSet2;
		}
		else
		{
			vertexSet--;
			for(int i = 1; i < length-1; ++i)
			{
				for(int j = 1; j < length-1; ++j)
				{
					colorSet1[i*length+j].r = 
						mk1*colorSet1[i*length+j].r  +
							mk2*colorSet2[i*length+j].r +
							mk3*(colorSet2[(i+1)*length+j].r + 
							     colorSet2[(i-1)*length+j].r + 
							     colorSet2[i*length+j+1].r + 
							     colorSet2[i*length+j-1].r);
				}
			}

			mesh.colors = colorSet1;
		}
	}
	
	public void disturbRandom()
	{
		int i = Random.Range(2,length - 2);
		int j = Random.Range(2,length - 2);
		
		//float half = .5f * disturbStrength;


		if(vertexSet == 1)
		{
			colorSet1[i*length+j].r += disturbStrength;
			colorSet1[i*length+j+1].r += disturbStrength;
			colorSet1[i*length+j-1].r += disturbStrength;
			colorSet1[(i+1)*length+j].r += disturbStrength;
			colorSet1[(i-1)*length+j].r += disturbStrength;
		}
		else
		{
			colorSet2[i*length+j].r += disturbStrength;
			colorSet2[i*length+j+1].r += disturbStrength;
			colorSet2[i*length+j-1].r += disturbStrength;
			colorSet2[(i+1)*length+j].r += disturbStrength;
			colorSet2[(i-1)*length+j].r += disturbStrength;
		}
	}

	//automaticly finds the closest vertex when given a world point
	public void disturbPoint(Vector3 point, int triangleIndex)
	{
		Vector2 vertex = NearestVertexTo(point, triangleIndex);

		//clamping - dont want to distrub an edge vertex due below
		int i = Mathf.Clamp((int)vertex.x,1,length-1);
		int j = Mathf.Clamp((int)vertex.y,1,length-1);

		if(vertexSet == 1)
		{
			colorSet1[i*length+j].r += disturbStrength;
			colorSet1[i*length+j+1].r += disturbStrength;
			colorSet1[i*length+j-1].r += disturbStrength;
			colorSet1[(i+1)*length+j].r += disturbStrength;
			colorSet1[(i-1)*length+j].r += disturbStrength;
		}
		else
		{
			colorSet2[i*length+j].r += disturbStrength;
			colorSet2[i*length+j+1].r += disturbStrength;
			colorSet2[i*length+j-1].r += disturbStrength;
			colorSet2[(i+1)*length+j].r += disturbStrength;
			colorSet2[(i-1)*length+j].r += disturbStrength;
		}
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

		return new Vector2(index/length,index%length);
	}
	
	private void solveTangents(Mesh mesh)
    {
            //speed up math by copying the mesh arrays
	    int[] triangles = mesh.triangles;
	    Vector3[] vertices = mesh.vertices;
	    Vector2[] uv = mesh.uv;
	    Vector3[] normals = mesh.normals;
	 
	    //variable definitions
	    int triangleCount = triangles.Length;
	    int vertexCount = vertices.Length;
	 
	    Vector3[] tan1 = new Vector3[vertexCount];
	    Vector3[] tan2 = new Vector3[vertexCount];
	 
	    Vector4[] tangents = new Vector4[vertexCount];
	 
	    for (long a = 0; a < triangleCount; a += 3)
	    {
	        long i1 = triangles[a + 0];
	        long i2 = triangles[a + 1];
	        long i3 = triangles[a + 2];
	 
	        Vector3 v1 = vertices[i1];
	        Vector3 v2 = vertices[i2];
	        Vector3 v3 = vertices[i3];
	 
	        Vector2 w1 = uv[i1];
	        Vector2 w2 = uv[i2];
	        Vector2 w3 = uv[i3];
	 
	        float x1 = v2.x - v1.x;
	        float x2 = v3.x - v1.x;
	        float y1 = v2.y - v1.y;
	        float y2 = v3.y - v1.y;
	        float z1 = v2.z - v1.z;
	        float z2 = v3.z - v1.z;
	 
	        float s1 = w2.x - w1.x;
	        float s2 = w3.x - w1.x;
	        float t1 = w2.y - w1.y;
	        float t2 = w3.y - w1.y;
	 
	        float r = 1.0f / (s1 * t2 - s2 * t1);
	 
	        Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
	        Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
	 
	        tan1[i1] += sdir;
	        tan1[i2] += sdir;
	        tan1[i3] += sdir;
	 
	        tan2[i1] += tdir;
	        tan2[i2] += tdir;
	        tan2[i3] += tdir;
	    }
	 
	 
	    for (long a = 0; a < vertexCount; ++a)
	    {
	        Vector3 n = normals[a];
	        Vector3 t = tan1[a];
	 
	        //Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
	        //tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
	        Vector3.OrthoNormalize(ref n, ref t);
	        tangents[a].x = t.x;
	        tangents[a].y = t.y;
	        tangents[a].z = t.z;
	 
	        tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
	    }
	 
	    mesh.tangents = tangents;
    }
}
