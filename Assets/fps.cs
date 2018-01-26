using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps : MonoBehaviour {


	private UnityEngine.UI.Text m_textTarget;

	void Start () {
		m_textTarget = GetComponent<UnityEngine.UI.Text>();
	}

	float time;
	float avgDelta;
	void Update()
	{
		avgDelta = (0.95f * avgDelta) + (0.05f * Time.deltaTime);

		m_textTarget.text = (avgDelta * 1000).ToString() + " ms";
	}
	
}
