using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MonsterHitLogger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            PlayerHealth ph = collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            // Debug.Log($"🐲 Player a touché le monstre {gameObject.name} via Trigger à {transform.position}");
        }
    }
}
