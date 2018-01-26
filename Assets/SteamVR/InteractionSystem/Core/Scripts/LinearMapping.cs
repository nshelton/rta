//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: A linear mapping value that is used by other components
//
//=============================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable] public class FloatEvent : UnityEvent<float> {}

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class LinearMapping : MonoBehaviour
	{

		[SerializeField]
		private float min = 0;
    	[SerializeField]
		private float max = 1;


		[SerializeField]
		private FloatEvent floatTarget;
		private LinearDrive m_drive;


		private TextMesh m_mesh;		

		private float m_normalizedValue;
		private float m_scaledValue;
		public float NormalizedValue
		{
			get {return m_normalizedValue;}
			set
			{
				m_normalizedValue = value;
				m_scaledValue = Mathf.Lerp(min, max, m_normalizedValue);
				
			}
		}

		public float ScaledValue
		{
			get {return m_scaledValue;}
			set
			{
				m_scaledValue = value;
				m_normalizedValue =  (m_scaledValue - min) / (max - min); 
			}
		}

		public float value;

		void Start()
		{
			m_mesh = transform.parent.GetComponentInChildren<TextMesh>();
			m_drive = transform.parent.GetComponent<LinearDrive>();
		}

		void Update()
		{
			
			m_scaledValue = Mathf.Lerp(min, max, m_normalizedValue);
			m_mesh.text = string.Format("{0}", m_scaledValue);
			
            floatTarget.Invoke(m_scaledValue);
		}
	}
}
