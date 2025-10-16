using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float m_TranslationSpeed;
    [SerializeField]
    private float m_JumpImpulsionMagnitude;

//    [SerializeField] private Transform m_camera;

    Rigidbody m_Rb;

    //private bool m_IsGrounded;

    private int m_GroundContacts = 0;

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        bool jump = Input.GetAxis("Jump") > 0 /*|| Input.GetKeyDown(KeyCode.Space)*/;

        Vector3 moveVect = transform.forward * m_TranslationSpeed * Time.deltaTime / 6.0f;
        m_Rb.MovePosition(m_Rb.position + moveVect);

        if (jump && m_GroundContacts > 0)
        {
            Vector3 jumpForce = Vector3.up * m_JumpImpulsionMagnitude;
            m_Rb.AddForce(jumpForce, ForceMode.Impulse);
        }

        if (jump)
        {
            Debug.Log("saut");
        }

        if (m_GroundContacts > 0)
        {
            m_Rb.linearVelocity = Vector3.zero;
            Debug.Log("aux sol");
        }

        m_Rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("touché");
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.LogError(Time.frameCount+" colLocalPt = " + colLocalPt+ "   colLocalPt.magnitude = "+ colLocalPt.magnitude);

            m_GroundContacts++;
            Debug.Log("Au sol (" + m_GroundContacts + ")");
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("quitter");
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_GroundContacts = Mathf.Max(0, m_GroundContacts - 1);
            Debug.Log("Quitter sol (" + m_GroundContacts + ")");
        }
    }

    void LateUpdate()
    {
//        if (m_camera != null)
//        {
//            Vector3 camPos = m_camera.position;
//            camPos.x = m_Rb.position.x + 5.0f;
//            m_camera.position = camPos;
//        }
    }
}
