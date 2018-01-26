using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITextControl : MonoBehaviour {

	private TextMesh  m_textMesh;

	[SerializeField]
	private MultibufferParticles m_target;

	[SerializeField]
	private SteamVR_TrackedController controller0;
	[SerializeField]
	private SteamVR_TrackedController controller1;

	void Start () {
		m_textMesh = GetComponent<TextMesh>();
	}

	void Update () {
		float scale = m_target.transform.localToWorldMatrix.GetScale().x;
		m_textMesh.text = string.Format("scale : {0}", scale);

	}


}
