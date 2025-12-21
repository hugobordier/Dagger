using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI m_ScoreText;

    [Header("Configuration Globale")]
    [SerializeField]
    private Player m_Player;

    [Header("Configuration des Coeurs")]
    [SerializeField]
    private GameObject m_HeartPrefab;

    [SerializeField]
    private Transform m_HeartContainer;

    // NOUVEAU : On définit combien de slot on affiche au total (ex: 3)
    [SerializeField]
    private int m_TotalHeartSlots = 3;

    [Header("Visuels")]
    [SerializeField]
    private Sprite m_FullHeartSprite; // Glisse ton image coeur rouge ici

    [SerializeField]
    private Sprite m_EmptyHeartSprite; // Glisse ton image coeur vide/gris ici

    private List<Image> m_HeartImages = new List<Image>(); // On stocke directement les composants Image

    void Start()
    {
        if (m_Player == null || m_HeartPrefab == null)
            return;
        // 1. On initialise SEULEMENT le nombre de slots visuels (3), pas la vie max du joueur (1000)
        InitializeHearts();
        // 2. Premier affichage
        UpdateHeartsDisplay();
    }

    void Update()
    {
        UpdateHeartsDisplay();
        UpdateScoreDisplay();
    }

    void InitializeHearts()
    {
        foreach (Transform child in m_HeartContainer)
        {
            if (child.gameObject != m_HeartPrefab)
                Destroy(child.gameObject);
        }

        m_HeartPrefab.SetActive(false);

        // création des slot de coeur (que 3 comme ça on peut 100000000000000000000 de coeur en facile :) )
        for (int i = 0; i < m_TotalHeartSlots; i++)
        {
            GameObject newHeartObj = Instantiate(m_HeartPrefab, m_HeartContainer);
            newHeartObj.SetActive(true);

            Image imgComponent = newHeartObj.GetComponent<Image>();
            if (imgComponent != null)
            {
                m_HeartImages.Add(imgComponent);
            }
        }
    }

    public void UpdateHeartsDisplay()
    {
        int currentHealth = m_Player.Health;

        for (int i = 0; i < m_HeartImages.Count; i++)
        {
            if (i < currentHealth)
            {
                m_HeartImages[i].sprite = m_FullHeartSprite;
            }
            else
            {
                m_HeartImages[i].sprite = m_EmptyHeartSprite;
            }
        }
    }

    void UpdateScoreDisplay()
    {
        if (m_ScoreText != null)
        {
            if (ScoreManager.Instance != null)
            {
                float score = ScoreManager.Instance.Precision;
                m_ScoreText.text = score.ToString("F1") + " %";
            }
            else
            {
                m_ScoreText.text = "-- %";
            }
        }
    }
}
