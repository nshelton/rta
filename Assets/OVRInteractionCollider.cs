using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRInteractionCollider : MonoBehaviour
{
    [SerializeField]
    public OVRTouchSample.TouchController Controller;


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter " + collision.gameObject.name);
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit " + collision.gameObject.name);

    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Stay " + collision.gameObject.name);

    }
}
