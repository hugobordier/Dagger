using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float m_TranslationSpeed;
    [SerializeField]
    // private float m_JumpImpulsionMagnitude;
    private float m_JumpHeight = 5f; // La hauteur du saut
    [SerializeField]
    private float m_HoverTime; // La durée en secondes passée en l'air
    private bool m_IsJumping = false; // Pour savoir si on est déjà en train de sauter

    [SerializeField] 
    private GameObject m_ColorZonePrefab;
    [SerializeField] 
    private float m_ZoneDistance = 1f; // distance devant le perso
    [SerializeField]
    private float m_ZoneLifetime = 0.5f; // durée d’affichage
    Coroutine jumpCoroutine;
    [SerializeField]
    private PlayerHealth m_PlayerHealth;
    private float m_CurrentJumpTimer = 0f;


    // [SerializeField] private Transform m_camera;
    Rigidbody2D m_Rb;

    //private bool m_IsGrounded;
    private int m_GroundContacts = 0;
    private bool k_pressed;

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        // Try to find PlayerHealth on the same gameObject if not set in inspector
        if (m_PlayerHealth == null)
            m_PlayerHealth = GetComponent<PlayerHealth>();
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
        // Debug.Log("m_GroundContacts = " + m_GroundContacts);

        HandleAttackInput();

        if (Input.GetKeyDown(KeyCode.K))
        {
            k_pressed = true;
        }
    }

    void FixedUpdate()
    {
        bool jump = Input.GetAxis("Jump") > 0 /*|| Input.GetKeyDown(KeyCode.Space)*/;

        // Vector2 moveVect = (Vector2)transform.right * m_TranslationSpeed * Time.deltaTime / 6.0f;
        // m_Rb.MovePosition(m_Rb.position + moveVect);
        m_Rb.linearVelocity = new Vector2(m_TranslationSpeed / 6.0f, m_Rb.linearVelocity.y);
        m_HoverTime = 60.0f / m_TranslationSpeed; // ajuster le temps de vol en fonction de la vitesse

        if (jump && m_GroundContacts > 0 && !m_IsJumping)
        {
            // Vector2 jumpForce = Vector2.up * m_JumpImpulsionMagnitude;
            // Debug.Log("Jump ! Force = " + jumpForce);
            // m_Rb.AddForce(jumpForce, ForceMode2D.Impulse);
            jumpCoroutine = StartCoroutine(TimedJump());
        }

        if (m_GroundContacts > 0)
        {
            //m_Rb.linearVelocity = Vector2.zero;
            // m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, 0);
            // Debug.Log("aux sol");
        }
        
        if (k_pressed)
        {
            k_pressed = false;
            Debug.Log("jumpCoroutine = " + jumpCoroutine);
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity);
                // Debug.Log("Raycast hit distance = " + hit.distance);
                if (hit.collider != null)
                {
                    // On se place sur le point trouvé
                    transform.position = new Vector2(transform.position.x, hit.point.y);
                }
                m_IsJumping = false;
                m_Rb.gravityScale = 1f;
            }
        }

        if (m_GroundContacts == 0 && !m_IsJumping) 
        {
            m_Rb.gravityScale = 10f; 
        }
        else if (m_IsJumping)
        {
            m_Rb.gravityScale = 0f;
        }
        else 
        {
            m_Rb.gravityScale = 1f;
        }

        m_Rb.angularVelocity = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x < -0.5f)
                {
                    Debug.Log("Mur détecté ! Grimpe !");
                    Collider2D wallCollider = collision.collider;
                    float newY = wallCollider.bounds.max.y + 0.02f;
                    transform.position = new Vector2(transform.position.x, newY);
                    return;
                }
                
            }
            //Debug.LogError(Time.frameCount+" colLocalPt = " + colLocalPt+ "   colLocalPt.magnitude = "+ colLocalPt.magnitude);

            m_GroundContacts++;
            //Debug.Log("Au sol (" + m_GroundContacts + ")");
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log("quitter");
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_GroundContacts = Mathf.Max(0, m_GroundContacts - 1);
            //Debug.Log("Quitter sol (" + m_GroundContacts + ")");
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

        m_CurrentJumpTimer = 0f;
        while (m_CurrentJumpTimer < m_HoverTime)
        {
            m_CurrentJumpTimer += Time.deltaTime;            
            yield return null; 
        }

        // On cherche le sol directement en dessous
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity);
        // Debug.Log("Raycast hit distance = " + hit.distance);

        if (hit.collider != null)
        {
            // On se place sur le point trouvé
            transform.position = new Vector2(transform.position.x, hit.point.y);
        }

        m_Rb.gravityScale = 1f;
        m_IsJumping = false;
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PerformAttack(Color.red);
        }
        else if (Input.GetKeyDown(KeyCode.S)) // 'else if' empêche de lancer 2 couleurs en même temps
        {
            PerformAttack(Color.green);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PerformAttack(Color.blue);
        }
    }

    void PerformAttack(Color color)
    {
        if (m_ColorZonePrefab == null) return;
        ExtendAirTime();

        // 1. Calcul de la position
        // Astuce : utiliser transform.right gère automatiquement le fait que le perso soit retourné ou non
        float spawnX = transform.position.x + m_ZoneDistance + 0.5f; 
        float spawnY = transform.position.y + 1; // Tu peux ajouter un offset ici genre : + 0.5f;

        Vector2 spawnPosition = new Vector2(spawnX, spawnY); 
        
        // 2. Instantiation
        GameObject attackObject = Instantiate(m_ColorZonePrefab, spawnPosition, Quaternion.identity);

        // 3. Hiérarchie (L'attaque suit le joueur)
        attackObject.transform.SetParent(transform);

        // 4. Configuration visuelle
        SpriteRenderer sr = attackObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color displayColor = color;
            displayColor.a = 0.5f; // Transparence
            sr.color = displayColor;
        }

        // 5. Configuration Logique (C'est là qu'on passe l'info à la Hitbox !)
        AttackZone zoneScript = attackObject.GetComponent<AttackZone>();
        if (zoneScript != null)
        {
            zoneScript.attackColor = color; // On "charge" l'attaque avec la bonne couleur
        }

        // 6. Nettoyage
        Destroy(attackObject, m_ZoneLifetime);
    }

    public void ExtendAirTime()
    {
        if (!m_IsJumping) return;

        float halfTime = m_HoverTime / 2f;

        if (m_CurrentJumpTimer > halfTime)
        {
            m_CurrentJumpTimer = halfTime;
            
            Debug.Log("Saut prolongé !");
        }
    }

}
