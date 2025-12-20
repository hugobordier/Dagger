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
    private int m_MaxHealth;
    private int m_CurrentHealth;
    public int Health => m_CurrentHealth;
    private float m_CurrentJumpTimer = 0f;
    private Animator m_Animator;

    // [SerializeField] private Transform m_camera;
    Rigidbody2D m_Rb;

    //private bool m_IsGrounded;
    private int m_GroundContacts = 0;
    private bool k_pressed;
    private bool m_CanControl_hole = true;
    private bool m_CanControl = false;

    public void EnableControl()
    {
        m_CanControl = true;
    }

    public void Stop()
    {
        m_CanControl = false;
        if (m_Rb != null)
            m_Rb.linearVelocity = Vector2.zero;
        if (m_Animator != null)
            m_Animator.SetBool("IsJumping", false); // Reset states if needed
    }

    void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<Animator>();
        m_StartX = transform.position.x;
        if (MenuManager.Instance != null)
        {
            if (MenuManager.Instance.IsEasyMode)
            {
                m_MaxHealth = 1000; // Mode God
            }
            else
            {
                m_MaxHealth = 3; // Mode Normal
            }
        }
        m_CurrentHealth = m_MaxHealth;
        // Try to find PlayerHealth on the same gameObject if not set in inspector
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (!m_CanControl_hole)
            return;
        if (!m_CanControl)
            return;

        // Debug.Log("m_GroundContacts = " + m_GroundContacts);

        HandleAttackInput();

        if (Input.GetKeyDown(KeyCode.K))
        {
            k_pressed = true;
        }
    }

    private float m_StartX;

    void FixedUpdate()
    {
        if (!m_CanControl)
            return;
        float currentSpeedX = m_TranslationSpeed / 6.0f;
        // Sync with Music Logic
        if (SoundPlayer.Instance != null && SoundPlayer.Instance.IsMusicPlaying)
        {
            float musicPositionSeconds = SoundPlayer.Instance.GetMusicPosition() / 1000f;
            float expectedX = m_StartX + (currentSpeedX * musicPositionSeconds);
            float currentX = m_Rb.position.x;
            float error = expectedX - currentX;
            float correction = error * 2.0f;
            m_Rb.linearVelocity = new Vector2(currentSpeedX + correction, m_Rb.linearVelocity.y);
        }
        else
        {
            m_Rb.linearVelocity = new Vector2(currentSpeedX, m_Rb.linearVelocity.y);
        }
        m_HoverTime = 60.0f / m_TranslationSpeed; // ajuster le temps de vol en fonction de la vitesse

        // if (m_GroundContacts > 0)
        // {
        //     //m_Rb.linearVelocity = Vector2.zero;
        //     // m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, 0);
        //     // Debug.Log("aux sol");
        // }

        if (k_pressed)
        {
            k_pressed = false;
            Debug.Log("jumpCoroutine = " + jumpCoroutine);
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
                m_Animator.SetBool("IsJumping", false);
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    Vector2.down,
                    Mathf.Infinity
                );
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
        if (!m_CanControl_hole)
            return;
        bool jump =
            Input.GetAxis("Jump")
            > 0 /*|| Input.GetKeyDown(KeyCode.Space)*/
        ;
        if (jump && m_GroundContacts > 0 && !m_IsJumping)
        {
            // Vector2 jumpForce = Vector2.up * m_JumpImpulsionMagnitude;
            // Debug.Log("Jump ! Force = " + jumpForce);
            // m_Rb.AddForce(jumpForce, ForceMode2D.Impulse);
            jumpCoroutine = StartCoroutine(TimedJump());
        }
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
                    Damage();
                    Collider2D wallCollider = collision.collider;
                    float newY = wallCollider.bounds.max.y + 0.02f;
                    transform.position = new Vector2(transform.position.x, newY);
                    return;
                }
            }
            m_GroundContacts++;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            StartCoroutine(DieSequence());
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
        if (m_Animator)
            m_Animator.SetBool("IsJumping", true);
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
            transform.position = new Vector2(transform.position.x, hit.point.y);
        }

        m_Rb.gravityScale = 1f;
        m_IsJumping = false;
        if (m_Animator)
            m_Animator.SetBool("IsJumping", false);
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PerformAttack(Color.red);
        }
        else if (Input.GetKeyDown(KeyCode.S))
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
        if (m_Animator != null)
        {
            if (color == Color.red)
                m_Animator.SetTrigger("TrigAttackR");
            else if (color == Color.green)
                m_Animator.SetTrigger("TrigAttackG");
            else if (color == Color.blue)
                m_Animator.SetTrigger("TrigAttackB");
        }

        if (m_ColorZonePrefab == null)
            return;
        ExtendAirTime();

        // float spawnX = transform.position.x + m_ZoneDistance + 0.5f;
        float spawnX = transform.position.x + m_ZoneDistance + 0.4f;
        float spawnY = transform.position.y + 1;

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        GameObject attackObject = Instantiate(
            m_ColorZonePrefab,
            spawnPosition,
            Quaternion.identity
        );

        attackObject.transform.SetParent(transform);

        SpriteRenderer sr = attackObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color displayColor = color;
            displayColor.a = 0.5f;
            sr.color = displayColor;
        }

        AttackZone zoneScript = attackObject.GetComponent<AttackZone>();
        if (zoneScript != null)
        {
            zoneScript.attackColor = color;
        }

        // 6. Nettoyage
        Destroy(attackObject, m_ZoneLifetime);
    }

    public void Damage()
    {
        m_CurrentHealth--;
        Debug.Log("Player damaged! Current health: " + m_CurrentHealth);
        if (m_Animator)
            m_Animator.SetTrigger("TrigDamage");

        if (m_CurrentHealth <= 0)
        {
            Debug.Log("Player is dead!");
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died. Game Over.");
        if (m_Animator && m_CanControl_hole)
        {
            m_Rb.linearVelocity = Vector2.zero;
            m_CanControl = false;
            m_Animator.SetTrigger("TrigDeath");
            StartCoroutine(GameOverDelay(1.0f));
        }
        else
        {
            // Si on ne peut plus contrôler le perso (chute dans trou), on skip l'anim de mort
            MenuManager.Instance.OpenGameoverMenu();
        }
    }

    public void ExtendAirTime()
    {
        if (!m_IsJumping)
            return;

        float halfTime = m_HoverTime / 2f;

        if (m_CurrentJumpTimer > halfTime)
        {
            m_CurrentJumpTimer = halfTime;
            Debug.Log("Saut prolongé !");
        }
    }

    IEnumerator DieSequence()
    {
        Debug.Log("Chute en cours...");
        m_CanControl_hole = false;
        yield return new WaitForSeconds(0.3f);
        Die();
    }

    IEnumerator GameOverDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MenuManager.Instance.OpenGameoverMenu();
    }
}
