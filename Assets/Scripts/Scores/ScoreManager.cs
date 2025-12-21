using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int killedEnemyNumber;
    private int totalEnemeyNumber;

    private float precision;
    public float Precision => precision;

    void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Found more than one ScoreManager instance in the scene");
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.precision = 0f;
        if (Judge.Instance != null)
        {
            Judge.Instance.EnemyKilled += IncreasePrecision;
        }
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyNotKilled += DecreasePrecision;
        }
    }

    void OnDestroy()
    {
        if (Judge.Instance != null)
        {
            Judge.Instance.EnemyKilled -= IncreasePrecision;
        }
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyNotKilled -= DecreasePrecision;
        }
    }

    // Update is called once per frame
    void Update() { }

    private void IncreasePrecision(int enemyBeat)
    {
        killedEnemyNumber++;
        totalEnemeyNumber++;
        precision = killedEnemyNumber * 100f / totalEnemeyNumber;
    }

    private void DecreasePrecision()
    {
        totalEnemeyNumber++;
        precision = killedEnemyNumber * 100f / totalEnemeyNumber;
    }
}
