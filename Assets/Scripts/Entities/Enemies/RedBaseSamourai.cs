using UnityEngine;

public class RedBaseSamourai : MonoBehaviour
{
    void Start() { }

    void Update() { }

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
