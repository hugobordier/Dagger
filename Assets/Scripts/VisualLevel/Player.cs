using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float m_TranslationSpeed;

    [SerializeField]
    // private float m_JumpImpulsionMagnitude;
    private float m_JumpHeight = 4f; // La hauteur du saut

    [SerializeField]
    private float m_HoverTimeMultiplier = 1.0f; // Multiplicateur de durée de vol

    [SerializeField]
    private float m_ExtensionAirTime = 0.5f; // Temps supplémentaire en l'air lors d'une attaque
    private float m_HoverTime; // La durée en secondes passée en l'air
    private bool m_IsJumping = false; // Pour savoir si on est déjà en train de sauter

    [SerializeField]
    private GameObject m_ColorZonePrefab;

    [SerializeField]
    private float m_ZoneDistance = 1f; // distance devant le perso

    [SerializeField]
    private float m_ZoneLifetime; // durée d’affichage
    Coroutine jumpCoroutine;
    private int m_MaxHealth;
    private int m_CurrentHealth;
    public int Health => m_CurrentHealth;
    private float m_CurrentJumpTimer = 0f;
    private Animator m_Animator;

    // [SerializeField] private Transform m_camera;
    Rigidbody2D m_Rb;
    private Collider2D m_Collider;

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
        m_Collider = GetComponent<Collider2D>();
        m_Animator = GetComponentInChildren<Animator>();
        m_StartX = transform.position.x;
        if (MenuManager.Instance != null)
        {
            if (MenuManager.Instance.IsEasyMode)
            {
                m_MaxHealth = 1000000; // Mode God
            }
            else
            {
                m_MaxHealth = 3; // Mode Normal
            }
        }
        m_CurrentHealth = m_MaxHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.CloseWindow += OnCloseWindow;
        }
    }

    private void OnDestroy()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.CloseWindow -= OnCloseWindow;
        }
    }

    private void OnCloseWindow(int beat)
    {
        m_HasAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_CanControl_hole)
            return;
        if (!m_CanControl)
            return;
        HandleAttackInput();
        if (Input.GetKeyDown(KeyCode.K))
        {
            k_pressed = true;
        }
    }

    private float m_StartX;
    private bool m_HasAttacked = false;

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
        m_HoverTime = (60.0f / m_TranslationSpeed) * m_HoverTimeMultiplier; // ajuster le temps de vol en fonction de la vitesse et du multiplicateur
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
                    Damage();
                    Collider2D wallCollider = collision.collider;
                    float newY = wallCollider.bounds.max.y + 0.02f;
                    transform.position = new Vector2(transform.position.x, newY);
                    return;
                }
            }
            m_GroundContacts++;
        }
        else if (
            collision.gameObject.CompareTag("BaseSamourai")
            || collision.gameObject.CompareTag("BaseBird")
        )
        {
            Damage();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            StartCoroutine(DieSequence());
        }
        else if (
            collision.gameObject.CompareTag("BaseSamourai")
            || collision.gameObject.CompareTag("BaseBird")
        )
        {
            if (m_Collider.IsTouching(collision))
            {
                Damage();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log("quitter");
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_GroundContacts = Mathf.Max(0, m_GroundContacts - 1);
        }
    }

    void LateUpdate() { }

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
        if (m_HasAttacked)
            return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            PerformAttack(Color.red);
            m_HasAttacked = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PerformAttack(Color.green);
            m_HasAttacked = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PerformAttack(Color.blue);
            m_HasAttacked = true;
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
        Destroy(attackObject, m_ZoneLifetime);
    }

    public void Damage()
    {
        m_CurrentHealth--;
        Debug.Log("Player damaged! Current health: " + m_CurrentHealth);
        if (m_CurrentHealth <= 0)
        {
            Referee.Instance.StopAudioPipeline();
            Debug.Log("Player is dead!");
            Die();
        }
        else
        {
            if (m_Animator)
                m_Animator.SetTrigger("TrigDamage");
        }
    }

    private void Die()
    {
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
        float ExtendAirTime = m_HoverTime * m_ExtensionAirTime;
        if (m_CurrentJumpTimer > ExtendAirTime)
        {
            m_CurrentJumpTimer = ExtendAirTime;
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
