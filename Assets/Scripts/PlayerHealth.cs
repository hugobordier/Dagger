using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vies")]
    public int maxLives = 3;
    public int currentLives;

    [Header("UI")]
    public PlayerUI playerUI; // glisse ton Canvas PlayerUI ici

    void Awake()
    {
        currentLives = maxLives;
    }

    public void TakeDamage(int amount = 1)
    {
        currentLives -= amount;
        currentLives = Mathf.Max(0, currentLives);
        // Mise à jour des cœurs
        if (playerUI != null)
        {
            playerUI.UpdateHearts(currentLives);
        }
        if (currentLives <= 0) { }
    }
}
