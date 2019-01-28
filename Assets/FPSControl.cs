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

    private bool m_isDragging;
    private Vector3 m_initialCamEuler;

    // Update is called once per frame
    void Update()
    {
 
         if (Input.GetMouseButton(1))
         {
            if ( !m_isDragging)
            {
                m_initialCamEuler = new Vector3(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y,
                    transform.eulerAngles.z);

                m_clickPos = Input.mousePosition;
                m_isDragging = true;
            }

             Vector3 p = Input.mousePosition;
                transform.eulerAngles = new Vector3(
                    m_initialCamEuler.x + (p.y - m_clickPos.y) * m_rotationSensitivity, 
                    m_initialCamEuler.y - (p.x - m_clickPos.x) * m_rotationSensitivity,
                    m_initialCamEuler.z);

         }
         else
         {
            m_isDragging = false;
         }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + transform.up * m_sensitivity;
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position - transform.up * m_sensitivity;
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
