/********************************************************************************//**
\file      TouchController.cs
\brief     Animating controller that updates with the tracked controller.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;
using UnityEngine.Events;

namespace OVRTouchSample
{
    public class TouchController : MonoBehaviour
    {
        [SerializeField]
        private OVRInput.Controller m_controller;

        [SerializeField]
        private Animator m_animator = null;

        public delegate void ControllerEvent();

        private bool m_restoreOnInputAcquired = false;

        private bool m_isGripped;

        public bool Gripped
        {
            get { return m_isGripped; }
        }

        public bool TriggerDown
        {
            get { return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > 0.5f; }
        }

        public event ControllerEvent OnGrip;
        public event ControllerEvent OnUngrip;

        public Vector2 Joystick
        {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller); }
        }

        private void Update()
        {
            bool currentGrip = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) > 0.1f;

            if (currentGrip != m_isGripped)
            {
                if (currentGrip && OnGrip != null)
                {
                    OnGrip.Invoke();
                }

                else if (!currentGrip && OnUngrip != null)
                {
                    OnUngrip.Invoke();
                }

                m_isGripped = currentGrip;
            }
        }

    }
}
