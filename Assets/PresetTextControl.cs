using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetTextControl : MonoBehaviour {

	private TextMesh  m_textMesh;
	private UnityEngine.UI.Text m_text;


	[SerializeField]
	private PresetController m_controller;

	void Start () {
		m_textMesh = GetComponent<TextMesh>();
		m_text = GetComponent<UnityEngine.UI.Text>();
	}

	void Update () {

		var str = string.Format("{0} / {1}", m_controller.currentPreset, m_controller.m_presets.Count);
		
		if ( m_text != null)
		{
			m_text.text = str;
		}

		if ( m_textMesh != null)
		{
			m_textMesh.text = str;
		}
	}


}
