using UnityEngine;

public class AttackZone : MonoBehaviour
{
    // On stocke la couleur (ou le type d'attaque) pour que l'ennemi sache par quoi il est touché
    public Color attackColor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.CompareTag("Enemy"))
        // {
        //     Debug.Log("Hit ! Ennemi touché par une attaque de couleur : " + attackColor);

        //     // ICI : On contactera le script de l'ennemi plus tard
        //     // Ex: collision.GetComponent<Enemy>().TakeDamage(attackColor);
        // }
    }
}

