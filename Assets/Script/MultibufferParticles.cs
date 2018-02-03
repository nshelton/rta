using UnityEngine;

public class MultibufferParticles : MonoBehaviour
{
	private struct Particle
	{
		public float x,y,z;
		public float r,g,b;
	}

	private const int SIZE_PARTICLE = 24;

	[SerializeField]
	private int particleCount = 10000;
	public float Particles { get{return particleCount;} set{particleCount = (int)value * 1000; ResetBuffers();} }
	private float mPointSize = 3;
	public float PointSize 
	{ 
		get { return mPointSize;} 
		set
		{
			mPointSize = value; 
			material.SetFloat("_Size", mPointSize);
		}
	}

	[SerializeField]
	public Material material;

	[SerializeField]
	public ComputeShader computeShader;

	private int mComputeShaderKernelID;
	private const int WARP_SIZE = 512;
	private int mWarpCount;

	[SerializeField]
	private Vector3 camPos;

	[SerializeField]
	private float _FOV = 1.0f;


	[SerializeField]
	private float _StepRatio = 1f;
	public float StepRatio { get{return _StepRatio;} set{_StepRatio = value;} }

	[SerializeField]
	private float _Threshold = 1f;
	public float Threshold { get{return _Threshold;} set{_Threshold = value;} }

	[SerializeField]
	private float _jitter = 1f;
	
	[SerializeField]
	private float _fogAmount = 1f;

 	[ColorUsageAttribute(true,true,0f,8f,0.125f,3f)] 
	[SerializeField]
	private Color _Color;

	[SerializeField]
	private int numBuffers = 128;

	[SerializeField]
	private int numActiveBuffers = 8;

	public float NumActiveBuffers
	{
		get {return numActiveBuffers;}
		set {numActiveBuffers = (int)value;}
	} 

	[SerializeField]
	private int activeBuffer = 0;

	private ComputeBuffer[] particleBuffers;

	public void ResetBuffers()
	{
		particleBuffers = new ComputeBuffer[numBuffers];
		mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);

		// make all our buffers
		for ( int i = 0; i < numBuffers; i ++)
		{
			Particle[] particleArray = new Particle[particleCount];
			particleBuffers[i] = new ComputeBuffer(particleCount, SIZE_PARTICLE);
			particleBuffers[i].SetData(particleArray);
		}
	}

	void Start()
	{
		mComputeShaderKernelID = computeShader.FindKernel("CSMain");
		ResetBuffers();
	}

	void OnDestroy()
	{
		for(int i = 0; i < particleBuffers.Length; i ++)
		{
			if ( particleBuffers[i] != null)
				particleBuffers[i].Release();
		}
	}
	
	void Update()
	{
		material.SetMatrix("modelToWorld", transform.localToWorldMatrix);
		material.SetColor("_Color", _Color);

		activeBuffer = (activeBuffer + 1) % numActiveBuffers;

		// Send datas to the compute shader
		computeShader.SetFloat("Time", Time.time / 40f);
		computeShader.SetFloat("FOV", _FOV);
		computeShader.SetFloat("pointDim", Mathf.Sqrt(particleCount));
		computeShader.SetFloat("stepRatio", _StepRatio);

		computeShader.SetVector("camPos",
		 transform.transform.InverseTransformPoint(Camera.main.transform.position));

		Quaternion q =  Quaternion.Inverse(transform.rotation) * Camera.main.transform.rotation;

		computeShader.SetVector("camQRot", new Vector4(q.x, q.y, q.z, q.w));
		computeShader.SetFloat("scale", transform.localToWorldMatrix.GetScale().x );

		computeShader.SetFloat("_Threshold", _Threshold);
		computeShader.SetFloat("_StepRatio", _StepRatio);
		computeShader.SetFloat("_jitter", _jitter);
		computeShader.SetFloat("_fogAmount", _fogAmount);

		// Update the Particles, only for the ative buffer
		computeShader.SetBuffer(mComputeShaderKernelID, "particleBuffer", particleBuffers[activeBuffer]);
		computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);
	}

	void OnRenderObject()
	{
		// draw only the active buffers
		for ( int i = 0; i < numActiveBuffers; i++)
		{
			material.SetPass(0);
			material.SetBuffer("particleBuffer", particleBuffers[i]);

			int wrap =  (i > activeBuffer) ? numActiveBuffers : 0;
			int age = wrap + activeBuffer - i ;
			float sizeFalloff = 1f; //- ((float)age / (float)numActiveBuffers);

			material.SetFloat("_Size", mPointSize * sizeFalloff);
			Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
		}
	}
	
}
