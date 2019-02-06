using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector4Shader : MonoBehaviour {

	[SerializeField]
	private ComputeShader m_target;

	[SerializeField]
	private Vector4 m_theValue;

	[SerializeField]
	private string m_shaderVarName;

	[SerializeField]
	private TextMesh m_label;

	[SerializeField]
	private positionToVec m_UI;

	public float X 
	{
		get{ return m_theValue.x;}
		set{ m_theValue.x = value;}
	}

	public float Y
	{
		get{ return m_theValue.y;}
		set{ m_theValue.y = value;}
	}

	public float Z 
	{
		get{ return m_theValue.z;}
		set{ m_theValue.z = value;}
	}

	public float W
	{
		get{ return m_theValue.w;}
		set{ m_theValue.w = value;}
	}

	[SerializeField]
	public Valve.VR.InteractionSystem.LinearMapping SliderX;

	[SerializeField]
	public Valve.VR.InteractionSystem.LinearMapping SliderY;

	[SerializeField]
	public Valve.VR.InteractionSystem.LinearMapping SliderZ;

	[SerializeField]
	public Valve.VR.InteractionSystem.LinearMapping SliderW;


	[SerializeField]
	public UnityEngine.UI.Slider unity_sliderX;

	[SerializeField]
	public UnityEngine.UI.Slider unity_sliderY;

	[SerializeField]
	public UnityEngine.UI.Slider unity_sliderZ;

	[SerializeField]
	public UnityEngine.UI.Slider unity_sliderW;


	public Vector4 Vec4
	{
		get{ return m_theValue;}
		set
		{

            if ((m_theValue - value).magnitude < 0.001)
                return;

			m_theValue.x = value.x;
			m_theValue.y = value.y;
			m_theValue.z = value.z;
			m_theValue.w = value.w;

			if (m_UI != null)
			{
				m_UI.Set(m_theValue);
			}
			else
			{
				SliderW.ScaledValue = value.w;
				SliderX.ScaledValue = value.x;
				SliderY.ScaledValue = value.y;
				SliderZ.ScaledValue = value.z;
			}

			if (unity_sliderX != null)
			{
				unity_sliderX.value = value.x;
			}

			if (unity_sliderY != null)
			{
				unity_sliderY.value = value.y;
			}

			if (unity_sliderZ != null)
			{
				unity_sliderZ.value = value.z;
			}

			if (unity_sliderW != null)
			{
				unity_sliderW.value = value.w;
			}

		}
	}



	// Update is called once per frame
	public void Update () {
		
		m_target.SetVector(m_shaderVarName, m_theValue);

		if ( m_label != null)
		{
			m_label.text = string.Format("({0},{1},{2})", X, Y, Z);
		}
	}
}
