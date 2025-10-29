using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MonsterHitLogger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            Debug.Log($"🐲 Player a touché le monstre {gameObject.name} à {transform.position}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            PlayerHealth ph = collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            Debug.Log($"🐲 Player a touché le monstre {gameObject.name} via Trigger à {transform.position}");
        }
    }
}
