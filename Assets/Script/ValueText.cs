using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueText : MonoBehaviour {

	private UnityEngine.UI.Text m_textTarget;

	[SerializeField]
	private string m_suffix;

	public float Value 
	{
		set { m_textTarget.text = value.ToString() + " " + m_suffix;}
	}


	void Start () {
		m_textTarget = GetComponent<UnityEngine.UI.Text>();
	}
	
}
