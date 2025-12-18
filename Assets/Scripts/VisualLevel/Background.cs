using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Paramètres de Parallaxe")]
    [Tooltip("0 = Bouge pas (Premier plan/Vite), 1 = Suit la caméra (Fond/Immobile)")]
    [Range(-0.2f, 1f)]
    public float parallaxEffect;

    [Header("Répétition Infinie")]
    public bool isInfinite = true;
    
    [Tooltip("Espace vide entre deux répétitions (utile pour les piliers)")]
    public float gapBetweenSprites = 0f;

    private Transform m_CameraTransform;
    private float m_StartPosX;
    private float m_OffsetY;
    private float m_SpriteLength;

    void Start()
    {
        m_CameraTransform = Camera.main.transform;
        
        m_StartPosX = transform.position.x;
        
        m_OffsetY = transform.position.y - m_CameraTransform.position.y;

        if (GetComponent<SpriteRenderer>() != null)
        {
            m_SpriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        
        m_SpriteLength += gapBetweenSprites;
    }

    void LateUpdate()
    {
        // --- GESTION X (Parallaxe) ---
        
        float temp = (m_CameraTransform.position.x * (1 - parallaxEffect));
        
        float dist = (m_CameraTransform.position.x * parallaxEffect);

        // --- GESTION Y (Suivi vertical) ---
        float newY = m_CameraTransform.position.y + m_OffsetY;

        transform.position = new Vector3(m_StartPosX + dist, newY, transform.position.z);

        if (isInfinite && m_SpriteLength > 0)
        {
            if (temp > m_StartPosX + m_SpriteLength)
            {
                m_StartPosX += m_SpriteLength;
            }
            else if (temp < m_StartPosX - m_SpriteLength)
            {
                m_StartPosX -= m_SpriteLength;
            }
        }
    }
}