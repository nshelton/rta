//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]


	public class Interactable3DVector : MonoBehaviour
	{

		[SerializeField]
		private Vector4Shader target;

		[SerializeField]
		private GameObject indicator;

		[ColorUsageAttribute(false, true)] 
		[SerializeField]
		public Color m_color;

		[ColorUsageAttribute(false, true)] 
        [SerializeField]
		public Color m_hoverColor;

		[ColorUsageAttribute(false, true)] 
        [SerializeField]
		public Color m_gripColor;

		private Material m_material;


		private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & ( ~Hand.AttachmentFlags.SnapOnAttach ) & ( ~Hand.AttachmentFlags.DetachOthers );

		//-------------------------------------------------
		void Awake()
		{
			m_material = indicator.GetComponent<Renderer>().material;
		}


		//-------------------------------------------------
		// Called when a Hand starts hovering over this object
		//-------------------------------------------------
		private void OnHandHoverBegin( Hand hand )
		{
			m_material.SetColor("_EmissionColor", m_hoverColor);
		}


		//-------------------------------------------------
		// Called when a Hand stops hovering over this object
		//-------------------------------------------------
		private void OnHandHoverEnd( Hand hand )
		{
			m_material.SetColor("_EmissionColor", m_color);

		}


		//-------------------------------------------------
		// Called every Update() while a Hand is hovering over this object
		//-------------------------------------------------
		private void HandHoverUpdate( Hand hand )
		{
			if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
			{
				if ( hand.currentAttachedObject != gameObject )
				{
					hand.HoverLock( GetComponent<Interactable>() );

					// Attach this object to the hand
					hand.AttachObject( gameObject, attachmentFlags );
				}
				else
				{
					// Detach this object from the hand
					hand.DetachObject( gameObject );

					// Call this to undo HoverLock
					hand.HoverUnlock( GetComponent<Interactable>() );

				}
			}
		}


		//-------------------------------------------------
		// Called when this GameObject becomes attached to the hand
		//-------------------------------------------------
		private void OnAttachedToHand( Hand hand )
		{
			m_material.SetColor("_EmissionColor", m_gripColor);

		}


		//-------------------------------------------------
		// Called when this GameObject is detached from the hand
		//-------------------------------------------------
		private void OnDetachedFromHand( Hand hand )
		{
			m_material.SetColor("_EmissionColor", m_hoverColor);
		}


		//-------------------------------------------------
		// Called every Update() while this GameObject is attached to the hand
		//-------------------------------------------------
		private void HandAttachedUpdate( Hand hand )
		{
			var p = transform.localPosition;
			
            target.Vec4 = new Vector4(p.x, p.y, p.z, 1.0f);
		}


		//-------------------------------------------------
		// Called when this attached GameObject becomes the primary attached object
		//-------------------------------------------------
		private void OnHandFocusAcquired( Hand hand )
		{
			
		}


		//-------------------------------------------------
		// Called when another attached GameObject becomes the primary attached object
		//-------------------------------------------------
		private void OnHandFocusLost( Hand hand )
		{
		}
	}
}
