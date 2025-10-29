using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vies")]
    public int maxLives = 3;
    private int currentLives;

    void Awake()
    {
        currentLives = maxLives;
    }

    public void TakeDamage(int amount = 1)
    {
        currentLives -= amount;
        currentLives = Mathf.Max(0, currentLives);

        Debug.Log($"💔 Player touché ! Vies restantes : {currentLives}");

        if (currentLives <= 0)
        {
            Debug.Log("☠️ Player est mort ! (log seulement pour l'instant)");
            // Ici tu pourras gérer la mort plus tard
        }
    }
}
