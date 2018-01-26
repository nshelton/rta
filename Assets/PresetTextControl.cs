using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetTextControl : MonoBehaviour {

	private TextMesh  m_textMesh;


	[SerializeField]
	private PresetController m_controller;

	void Start () {
		m_textMesh = GetComponent<TextMesh>();
	}

	void Update () {
		m_textMesh.text = string.Format("{0} / {1}", m_controller.currentPreset, m_controller.m_presets.Count);
	}


}
