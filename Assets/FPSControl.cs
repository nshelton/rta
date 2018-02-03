using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSControl : MonoBehaviour
{

    [SerializeField]
    private float m_sensitivity = 0.01f;

    [SerializeField]
    private float m_rotationSensitivity = 0.01f;

    [SerializeField]
    private float m_rollSensitivity = 0.01f;

    [SerializeField]
    private float m_scaleSensitivity = 0.01f;

    [SerializeField]
    private GameObject m_scaleTarget;

    private Vector3 m_clickPos;


    // Update is called once per frame
    void Update()
    {
 
        // if (Input.GetMouseButton(1))
        // {
        //     Vector3 p = Input.mousePosition;
        //     Vector3 dir = new Vector3(-p.y + Screen.height / 2, p.x - Screen.width / 2, 0) / Screen.height;

        //     transform.Rotate(dir * m_rotationSensitivity);
        // }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, 0, m_rollSensitivity);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, 0, -m_rollSensitivity);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + transform.forward * m_sensitivity;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position - transform.right * m_sensitivity;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position - transform.forward * m_sensitivity;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + transform.right * m_sensitivity;
        }


    }
}
