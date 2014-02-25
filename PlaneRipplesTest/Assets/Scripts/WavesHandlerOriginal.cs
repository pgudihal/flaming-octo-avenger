using UnityEngine;
using System.Collections;

public class WavesHandlerOriginal : MonoBehaviour 
{
	private Vector3[] newVertices1;
	private Vector3[] newVertices2;
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
	public float disturbStrength = .5f;
	private float mk1,mk2,mk3;
	private float timer;
	private float timeStep;
	private float timeTillNextUpdate;
	
	private Mesh tempMesh;

	private float max = 0;
	private float min = 0;
	 
	void Start () 
	{
		//creating vertices
		newVertices1 = new Vector3[length*length];
		newVertices2 = new Vector3[length*length];
		float halfway = (length-1)*dx*.5f;
		for(int i = 0; i < length; ++i)
		{
			float z = halfway - i*dx + yCenter-dx;
			for(int j = 0; j < length; ++j)
			{
				float x = -halfway + j*dx + xCenter-dx;
				newVertices1[i*length+j] = new Vector3(x,0,z);
				newVertices2[i*length+j] = new Vector3(x,0,z);
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
		Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices1;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
		
		mesh.RecalculateNormals();
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		
		//waves vars
		float d = damping*Time.deltaTime + 2;
		float e = (speed*speed)*(Time.deltaTime*Time.deltaTime)/(dx*dx);
		mk1 = (damping*Time.deltaTime)/ d;
		mk2 = (2.0f-4.0f*e) / d;
		mk3 = 2*e / d;
		
		timeStep = .016667f;
		timeTillNextUpdate = timeStep + Time.timeSinceLevelLoad;
		timer = 0;
		
		tempMesh = new Mesh();
	}
	
	void Update () 
	{
		timer += Time.deltaTime;
		if(timer < timeTillNextUpdate) return;
		timeTillNextUpdate = timeStep + Time.timeSinceLevelLoad;
		
		float d = damping*timeStep+2.0f;
		float e = (speed*speed)*(timeStep*timeStep)/(dx*dx);
		mk1 = (damping*timeStep-2.0f)/ d;
		mk2 = (4.0f-8.0f*e) / d;
		mk3 = (2.0f*e) / d;
		
		if(Input.GetKeyDown(KeyCode.Space))disturbRandom();
		
		if(vertexSet == 1)
		{
			vertexSet++;//next time use vertex set 2
			for(int i = 1; i < length-1; ++i)
			{
				for(int j = 1; j < length-1; ++j)
				{
					newVertices2[i*length+j].y = 
						mk1*newVertices2[i*length+j].y +
						mk2*newVertices1[i*length+j].y +
						mk3*(newVertices1[(i+1)*length+j].y + 
						     newVertices1[(i-1)*length+j].y + 
						     newVertices1[i*length+j+1].y + 
							 newVertices1[i*length+j-1].y);

					if(newVertices2[i*length+j].y > max) max = newVertices2[i*length+j].y;
					if(newVertices2[i*length+j].y < min) min = newVertices2[i*length+j].y;
				}
			}
			
			//Mesh m = tempMesh;
			tempMesh.Clear();
			tempMesh.vertices = newVertices2;
			tempMesh.uv = newUV;
			tempMesh.triangles = newTriangles;
			tempMesh.RecalculateNormals();
			solveTangents(tempMesh);
			
			GetComponent<MeshFilter>().mesh.normals = tempMesh.normals;
			GetComponent<MeshFilter>().mesh.tangents = tempMesh.tangents;
		}
		else
		{
			vertexSet--;
			for(int i = 1; i < length-1; ++i)
			{
				for(int j = 1; j < length-1; ++j)
				{
					newVertices1[i*length+j].y = 
						mk1*newVertices1[i*length+j].y +
						mk2*newVertices2[i*length+j].y +
						mk3*(newVertices2[(i+1)*length+j].y + 
						     newVertices2[(i-1)*length+j].y + 
						     newVertices2[i*length+j+1].y + 
							 newVertices2[i*length+j-1].y);

					if(newVertices1[i*length+j].y > max) max = newVertices1[i*length+j].y;
					if(newVertices1[i*length+j].y < min) min = newVertices1[i*length+j].y;
				}
			}
			
			//Mesh m = tempMesh;
			tempMesh.Clear();
			tempMesh.vertices = newVertices1;
			tempMesh.uv = newUV;
			tempMesh.triangles = newTriangles;
			tempMesh.RecalculateNormals();
			solveTangents(tempMesh);
			
			GetComponent<MeshFilter>().mesh.normals = tempMesh.normals;
			GetComponent<MeshFilter>().mesh.tangents = tempMesh.tangents;
		}

		Debug.Log("MAX: " + max + " ::: MIN: " + min);
	}
	
	public void disturbRandom()
	{
		int i = Random.Range(2,length - 2);
		int j = Random.Range(2,length - 2);
		
		//float half = .5f * disturbStrength;
		
		//i = 100;
		//j = 100;
		
		if(vertexSet == 1)
		{
			newVertices1[i*length+j].y += disturbStrength;
			newVertices1[i*length+j+1].y += disturbStrength;
			newVertices1[i*length+j-1].y += disturbStrength;
			newVertices1[(i+1)*length+j].y += disturbStrength;
			newVertices1[(i-1)*length+j].y += disturbStrength;
		}
		else
		{
			newVertices2[i*length+j].y += disturbStrength;
			newVertices2[i*length+j+1].y += disturbStrength;
			newVertices2[i*length+j-1].y += disturbStrength;
			newVertices2[(i+1)*length+j].y += disturbStrength;
			newVertices2[(i-1)*length+j].y += disturbStrength;
		}
	}
	
	public void disturbVertex(Vector2 v, float disturbStrength)
	{
		int j = Mathf.FloorToInt(v.x);
		int i = Mathf.FloorToInt(v.y);
		//Debug.Log("i: " + i + "j: " + j);
		
		if(vertexSet == 1)
		{
			newVertices1[i*length+j].y += disturbStrength;
			newVertices1[i*length+j+1].y += disturbStrength;
			newVertices1[i*length+j-1].y += disturbStrength;
			newVertices1[(i+1)*length+j].y += disturbStrength;
			newVertices1[(i-1)*length+j].y += disturbStrength;
		}
		else
		{
			newVertices2[i*length+j].y += disturbStrength;
			newVertices2[i*length+j+1].y += disturbStrength;
			newVertices2[i*length+j-1].y += disturbStrength;
			newVertices2[(i+1)*length+j].y += disturbStrength;
			newVertices2[(i-1)*length+j].y += disturbStrength;
		}
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
