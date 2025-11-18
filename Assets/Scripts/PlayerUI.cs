using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Hearts UI")]
    public Image[] hearts;     
    public Sprite emptyHeart;  

    /// <summary>
    /// Met à jour les cœurs selon le nombre de vies restant.
    /// </summary>
    /// <param name="currentLives">Vies actuelles du joueur</param>
    public void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLives)
                hearts[i].sprite = hearts[i].sprite; // reste plein (optionnel : tu peux avoir un sprite plein dédié)
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
