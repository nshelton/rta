//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class EventButton : MonoBehaviour
	{
        [SerializeField]
   		public UnityEvent onClick;


		[SerializeField]
		private GameObject indicator;

		[ColorUsageAttribute(true,true)] 
		[SerializeField]
		public Color m_color;

		[ColorUsageAttribute(true,true)] 
		[SerializeField]
		public Color m_hoverColor;

		[ColorUsageAttribute(true,true)] 
		[SerializeField]
		public Color m_clickColor;

		private Material m_material;

        private bool m_canTriggerEvents = true;

		//-------------------------------------------------
		void Awake()
		{
			m_material = indicator.GetComponent<Renderer>().material;
			m_material.SetColor("_EmissionColor", m_color);
		}


        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------

        public bool IsHovering = false;
		public void OnHandHoverBegin( Hand hand )
		{
            IsHovering = true;
            m_material.SetColor("_EmissionColor", m_hoverColor);
		}


        //-------------------------------------------------
        // Called when a Hand stops hovering over this object
        //-------------------------------------------------
        public void OnHandHoverEnd( Hand hand )
		{
            IsHovering = false;
            m_material.SetColor("_EmissionColor", m_color);
            m_canTriggerEvents = true;
		}

        //-------------------------------------------------
        // Called every Update() while a Hand is hovering over this object
        //-------------------------------------------------
        public void HandHoverUpdate( Hand hand )
		{
			if (hand.GetStandardInteractionButtonDown() || 
            ( ( hand.controller != null ) && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
			{
				if (m_canTriggerEvents)
				{
                    m_canTriggerEvents = false;
			        m_material.SetColor("_EmissionColor", m_clickColor);
                    onClick.Invoke();
				}
			}
		}

        public void TriggerClick()
        {
            m_canTriggerEvents = false;
            m_material.SetColor("_EmissionColor", m_clickColor);
            onClick.Invoke();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name == "InteractionPoint")
                OnHandHoverEnd(null);

        }


    }
}
