using UnityEngine;

public class GreenBaseSamourai : MonoBehaviour
{
    public int beat;
    private bool hasPassedPlayer = false;

    void Start() { }

    void Update()
    {
        if (!hasPassedPlayer && EnemyManager.Instance != null && EnemyManager.Instance.player != null)
        {
            if (transform.position.x < EnemyManager.Instance.player.transform.position.x)
            {
                hasPassedPlayer = true;
                EnemyManager.Instance.TriggerEnemyNotKilled();
            }
        }
    }

    void OnDisable()
    {
        hasPassedPlayer = false;
    }

    public void Die()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ReturnEnemy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
