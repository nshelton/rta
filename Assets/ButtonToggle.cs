using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToggle : MonoBehaviour {

	[SerializeField]
	private GameObject[] m_targets;


	[SerializeField]
	private Valve.VR.InteractionSystem.EventButton m_control;
	[SerializeField]
	private Color m_activationColor = Color.yellow;

	[SerializeField]
	private bool m_moveObject = false;
	private Color m_normalColor;

	private void Start()
	{
		m_normalColor = m_control.m_color;
		foreach(GameObject obj in m_targets)
			obj.SetActive(false);
		
	}

	public void Toggle()
	{
		foreach(GameObject obj in m_targets)
		{
			obj.SetActive(!obj.activeSelf);
			if(m_moveObject)
			{
				obj.transform.position = transform.position;
				obj.transform.rotation = transform.rotation;
			}
		}
		
		if ( m_targets.Length > 0 && m_targets[0].activeSelf)
			m_control.m_color = m_activationColor;
		else 
			m_control.m_color = m_normalColor;

	}
}
