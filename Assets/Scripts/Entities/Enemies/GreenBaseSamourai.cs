using UnityEngine;

public class GreenBaseSamourai : MonoBehaviour
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
