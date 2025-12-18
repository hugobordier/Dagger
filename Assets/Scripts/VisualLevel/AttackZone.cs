using UnityEngine;

public class AttackZone : MonoBehaviour
{
    // On stocke la couleur (ou le type d'attaque) pour que l'ennemi sache par quoi il est touché
    public Color attackColor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackColor == Color.red)
        {
            RedBaseSamourai enemy = collision.GetComponent<RedBaseSamourai>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
        if (attackColor == Color.green)
        {
            GreenBaseSamourai enemy = collision.GetComponent<GreenBaseSamourai>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
        if (attackColor == Color.blue)
        {
            BlueBaseSamourai enemy = collision.GetComponent<BlueBaseSamourai>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
    }
}
