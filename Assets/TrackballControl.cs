using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackballControl : MonoBehaviour {

	[SerializeField]
	public SteamVR_TrackedController m_leftHand;

	[SerializeField]
    public SteamVR_TrackedController m_rightHand;

    [SerializeField]
    public OVRTouchSample.TouchController m_leftOculusController;

    [SerializeField]
    public OVRTouchSample.TouchController m_rightOculusController;

    [SerializeField]
    public GameObject m_cameraObject;

	public GameObject m_leftHandObject;

	[SerializeField]
    public GameObject m_rightHandObject;

	[SerializeField]
	public GameObject m_targetObject;

	[SerializeField]
	public GameObject m_rotateNode;

	[SerializeField]
	public GameObject m_TranslateNode;

	[SerializeField]
	public GameObject m_scaleNode;

	[SerializeField]
	public float m_flySpeed = 0.01f;

	public bool m_isGripping = false;

	public Vector3 m_startVector;

    [SerializeField]
    public InputMode m_inputMode;


	private void UpdateTrackball()
	{
		m_TranslateNode.transform.position	= (m_leftHandObject.transform.position + m_rightHandObject.transform.position) / 2f ;

		Vector3 endVector = (m_leftHandObject.transform.position - m_rightHandObject.transform.position);
			
		float currentScale = Vector3.Magnitude(endVector);

		m_rotateNode.transform.rotation = Quaternion.FromToRotation(m_startVector, endVector);
		m_scaleNode.transform.localScale = Vector3.one * currentScale / 2f;

	}

	private void StartGrip()
	{
		m_TranslateNode.SetActive(true);
		m_startVector = m_leftHandObject.transform.position - m_rightHandObject.transform.position;
		
		UpdateTrackball();

		m_targetObject.transform.SetParent(m_scaleNode.transform, true);
		m_isGripping = true;
	}

	private void EndGrip()
	{
		m_TranslateNode.SetActive(false);
		m_isGripping = false;
		m_targetObject.transform.SetParent(null, true);
	}

    // Vive callbacks
	void GrippedL(object sender, ClickedEventArgs e)
	{
		if (m_rightHand.gripped) StartGrip();
		
	}
	void GrippedR(object sender, ClickedEventArgs e)
	{
		if (m_leftHand.gripped) StartGrip();
	}

	void Ungripped(object sender, ClickedEventArgs e)
	{
		if( m_isGripping) EndGrip();
	}

    /// Oculus Callbacks
    void GrippedLOculus()
    {
        if (m_rightOculusController.Gripped) StartGrip();

    }
    void GrippedROculus()
    {
        if (m_leftOculusController.Gripped) StartGrip();
    }

    void UngrippedOculus()
    {
        if (m_isGripping) EndGrip();
    }

    void Start () {

        if (m_inputMode == InputMode.Vive)
        {
            m_leftHand.Gripped += GrippedL;
            m_rightHand.Gripped += GrippedR;

            m_leftHand.Ungripped += Ungripped;
            m_rightHand.Ungripped += Ungripped;
        }

        if (m_inputMode == InputMode.Oculus)
        {
            m_leftOculusController.OnGrip += GrippedLOculus;
            m_rightOculusController.OnGrip += GrippedROculus;

            m_leftOculusController.OnUngrip += UngrippedOculus;
            m_rightOculusController.OnUngrip += UngrippedOculus;
        }
	}

    void VelocityFromPad(OVRTouchSample.TouchController hand)
    {
        if (hand.Joystick.SqrMagnitude() > 0.001f)
        {
            Vector3 dir = Mathf.Pow(hand.Joystick.x, 3f) * hand.transform.right
                + Mathf.Pow(hand.Joystick.y, 3f) * hand.transform.forward;

            m_targetObject.transform.Translate(
                -m_targetObject.transform.InverseTransformDirection(dir) * m_flySpeed);
        }
    }


    void VelocityFromPad(SteamVR_TrackedController hand)
	{
			if(hand.padTouched)
			{
				Vector3 dir =  Mathf.Pow(hand.controllerState.rAxis0.x, 3f) * hand.transform.right
					+ Mathf.Pow(hand.controllerState.rAxis0.y, 3f)  * hand.transform.forward;

				m_targetObject.transform.Translate(
					-m_targetObject.transform.InverseTransformDirection(dir) * m_flySpeed);
			}
	}

	void GazeTrigger(SteamVR_TrackedController handr, SteamVR_TrackedController handl)
	{
			if(handr.triggerPressed || handl.triggerPressed)
			{
				Vector3 dir = m_cameraObject.transform.forward * m_flySpeed * 0.3f;

				m_targetObject.transform.Translate(
					-m_targetObject.transform.InverseTransformDirection(dir) );
			}
	}

	void Update () {
		if ( m_isGripping )
		{
			if (m_inputMode == InputMode.Vive)
            {
                if (!m_leftHand.gripped || !m_rightHand.gripped)
                {
                    EndGrip();
                    return;
                }
            }
            else if (m_inputMode == InputMode.Oculus)
            {
                if (!m_leftOculusController.Gripped || !m_rightOculusController.Gripped)
                {
                    EndGrip();
                    return;
                }
            }

			UpdateTrackball();
		}
		else
		{
			if (m_inputMode == InputMode.Vive)
            {
                VelocityFromPad(m_rightHand);
                VelocityFromPad(m_leftHand);
            }
            if (m_inputMode == InputMode.Oculus)
            {
                VelocityFromPad(m_rightOculusController);
                VelocityFromPad(m_leftOculusController);
            }

        }
	}


    public enum InputMode
    {
        Oculus,
        Vive
    }
}
