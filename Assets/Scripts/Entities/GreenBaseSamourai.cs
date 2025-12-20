using UnityEngine;

public class GreenBaseSamourai : MonoBehaviour
{
    public int beat;

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
