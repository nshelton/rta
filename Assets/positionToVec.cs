using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionToVec : MonoBehaviour {

	// Use this for initialization

	[SerializeField]
	private Vector4Shader target;


	[SerializeField]
	private GameObject parent;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// target.Vec3 =  parent.transform.worldToLocalMatrix * parent.transform.position;
		target.Vec4 = parent.transform.InverseTransformPoint(transform.position);

	}

	public void Set(Vector4 pos) {
        var result = parent.transform.TransformPoint(pos);

        if ((result - transform.position).magnitude > 0.001f)
        {
            transform.position = result;
            target.Vec4 = parent.transform.InverseTransformPoint(transform.position);
            target.Update();
        }
    }
}
