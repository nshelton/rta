using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class linecontroller : MonoBehaviour {

	[SerializeField]
	Transform p0;

	[SerializeField]
	Transform p1;


	LineRenderer m_lineRenderer;
	// Use this for initialization
	void Start () {
		m_lineRenderer = GetComponent<LineRenderer>();
		m_lineRenderer.positionCount = 2;
	}
	
	// Update is called once per frame
	void Update () {
		m_lineRenderer.SetPosition(0,p0.transform.position);
		m_lineRenderer.SetPosition(1,p1.transform.position);
	}
}
