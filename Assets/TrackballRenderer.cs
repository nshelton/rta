using UnityEngine;

public class TrackballRenderer : MonoBehaviour
{
	private struct Particle
	{
		public Vector3 position;
		public Vector3 color;
	}

	private const int SIZE_PARTICLE = 24;

	[SerializeField]
	private Material material;

 	[ColorUsageAttribute(true,true,0f,8f,0.125f,3f)] 
	[SerializeField]
	public Mesh mesh;
 
	void Update()
	{
		Graphics.DrawMesh(mesh, transform.position, transform.rotation, material, 0);
	}
 
	
}
