using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float m_TranslationSpeed;
    [SerializeField]
    // private float m_JumpImpulsionMagnitude;
    private float m_JumpHeight = 5f; // La hauteur du saut
    [SerializeField]
    private float m_HoverTime = 1f; // La durée en secondes passée en l'air
    private bool m_IsJumping = false; // Pour savoir si on est déjà en train de sauter

    // [SerializeField] private Transform m_camera;
    Rigidbody2D m_Rb;

    //private bool m_IsGrounded;
    private int m_GroundContacts = 0;

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Jump") > 0)
        {
            Debug.Log("Touche Saut pressée");
        }
        Debug.Log("m_GroundContacts = " + m_GroundContacts);
    }

    void FixedUpdate()
    {
        bool jump = Input.GetAxis("Jump") > 0 /*|| Input.GetKeyDown(KeyCode.Space)*/;

        // Vector2 moveVect = (Vector2)transform.right * m_TranslationSpeed * Time.deltaTime / 6.0f;
        // m_Rb.MovePosition(m_Rb.position + moveVect);
        m_Rb.linearVelocity = new Vector2(m_TranslationSpeed / 6.0f, m_Rb.linearVelocity.y);

        if (jump && m_GroundContacts > 0 && !m_IsJumping)
        {
            // Vector2 jumpForce = Vector2.up * m_JumpImpulsionMagnitude;
            // Debug.Log("Jump ! Force = " + jumpForce);
            // m_Rb.AddForce(jumpForce, ForceMode2D.Impulse);
            StartCoroutine(TimedJump());
        }

        if (m_GroundContacts > 0)
        {
            //m_Rb.linearVelocity = Vector2.zero;
            // m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, 0);
            Debug.Log("aux sol");
        }

        m_Rb.angularVelocity = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("touché");
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.LogError(Time.frameCount+" colLocalPt = " + colLocalPt+ "   colLocalPt.magnitude = "+ colLocalPt.magnitude);

            m_GroundContacts++;
            Debug.Log("Au sol (" + m_GroundContacts + ")");
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
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

    IEnumerator TimedJump()
    {
        m_IsJumping = true;
        m_GroundContacts = 0; 
        m_Rb.gravityScale = 0f;
        m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, 0);

        transform.position += Vector3.up * m_JumpHeight;

        yield return new WaitForSeconds(m_HoverTime);

        // On cherche le sol directement en dessous
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity);
        Debug.Log("Raycast hit distance = " + hit.distance);

        if (hit.collider != null)
        {
            // On se place sur le point trouvé
            transform.position = new Vector2(transform.position.x, hit.point.y);
        }

        m_Rb.gravityScale = 1f;
        m_IsJumping = false;
    }

}
