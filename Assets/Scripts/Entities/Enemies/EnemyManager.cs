using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public float bpm;

    [Header("Configuration")]
    [Tooltip("Name of the file in Resources/LevelData (without extension)")]
    public string levelFileName = "enemylevel1";

    [Tooltip(
        "Must match the Player's actual translation speed (e.g., Player.m_TranslationSpeed * 0.25f)"
    )]
    public float playerSpeed = 60;

    [Tooltip("List of enemy prefabs. Index corresponds to 'type' in the level file.")]
    public List<GameObject> enemyPrefabs;
    public Transform enemyParent;

    [Header("Pooling Settings")]
    public int initialPoolSizePerType = 10;
    public float spawnAheadDistance = 30f; // Distance ahead of camera to activate enemies
    public float despawnBehindDistance = 15f; // Distance behind camera to deactivate

    private Transform playerTransform;
    private List<EnemySpawnData> allEnemiesData = new List<EnemySpawnData>();
    private Dictionary<int, Queue<GameObject>> enemyPools =
        new Dictionary<int, Queue<GameObject>>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int nextEnemyIndex = 0;

    // Data structure to hold parsed file info
    private struct EnemySpawnData
    {
        public float beat;
        public int type;
        public float xPosition;
    }

    void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Found more than one EnemyManager instance in the scene");
        }
        Instance = this;
    }

    void Start()
    {
        this.bpm = Metronome.Instance.Bpm;
        this.playerSpeed = playerSpeed * 1.5f / 6.0f;
        Player player = FindObjectOfType<Player>(); // Unity 2023+
        if (player == null)
        {
            player = FindObjectOfType<Player>(); // Fallback
        }
        if (player != null)
        {
            playerTransform = player.transform;
            // NOTE: Verify if you want to auto-fetch speed:
            // playerSpeed = player.m_TranslationSpeed * 0.25f;
        }
        else
        {
            Debug.LogWarning(
                "EnemyManager: Player not found! Spawning logic based on position might fail."
            );
        }
        LoadLevelData();
        InitializePools();
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        float playerX = playerTransform.position.x;
        float spawnThreshold = playerX + spawnAheadDistance;
        float despawnThreshold = playerX - despawnBehindDistance;

        // 1. Spawn enemies that are coming into view
        while (
            nextEnemyIndex < allEnemiesData.Count
            && allEnemiesData[nextEnemyIndex].xPosition <= spawnThreshold
        )
        {
            SpawnEnemy(allEnemiesData[nextEnemyIndex]);
            nextEnemyIndex++;
        }

        // 2. Despawn enemies that have passed out of view
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemies[i];
            if (enemy.transform.position.x < despawnThreshold)
            {
                ReturnEnemyToPool(enemy);
                activeEnemies.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Loads level data from Resources/LevelData/[levelFileName].txt
    /// Format: "Beat;Type" per line.
    /// </summary>
    void LoadLevelData()
    {
        TextAsset file = Resources.Load<TextAsset>($"LevelData/{levelFileName}");
        if (file == null)
        {
            Debug.LogError(
                $"EnemyManager: Level file 'LevelData/{levelFileName}' not found in Resources."
            );
            return;
        }
        string[] lines = file.text.Split(
            new[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
        foreach (string line in lines)
        {
            // Format: Beat;Type
            string[] parts = line.Split(';');
            if (parts.Length >= 2)
            {
                if (
                    float.TryParse(parts[0], out float beat) && int.TryParse(parts[1], out int type)
                )
                {
                    float xPos = CalculatePosition(beat);
                    allEnemiesData.Add(
                        new EnemySpawnData
                        {
                            beat = beat,
                            type = type,
                            xPosition = xPos,
                        }
                    );
                }
            }
        }
        // Sort by position ensures the loop in Update works correctly
        // allEnemiesData.Sort((a, b) => a.xPosition.CompareTo(b.xPosition));
        Debug.Log($"EnemyManager: Loaded {allEnemiesData.Count} enemies.");
    }

    float CalculatePosition(float beat)
    {
        return playerSpeed * beat * (60f / bpm);
    }

    void InitializePools()
    {
        // Initialize a queue for each prefab type we have
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            enemyPools[i] = new Queue<GameObject>();

            // Pre-instantiate some objects
            for (int k = 0; k < initialPoolSizePerType; k++)
            {
                GameObject obj = CreateEnemy(i);
                obj.SetActive(false);
                enemyPools[i].Enqueue(obj);
            }
        }
    }

    GameObject CreateEnemy(int typeIndex)
    {
        if (typeIndex < 0 || typeIndex >= enemyPrefabs.Count)
            return null;
        GameObject prefab = enemyPrefabs[typeIndex];
        GameObject obj = Instantiate(prefab, enemyParent);
        // Tag/Component setup if needed
        EnemyIdentity id = obj.GetComponent<EnemyIdentity>();
        if (id == null)
            id = obj.AddComponent<EnemyIdentity>();
        id.typeIndex = typeIndex;

        return obj;
    }

    void SpawnEnemy(EnemySpawnData data)
    {
        if (data.type < 0 || data.type >= enemyPrefabs.Count)
            return;
        GameObject enemy = GetFromPool(data.type);
        if (enemy != null)
        {
            // Position the enemy. Assuming Y is 0 or handled by the prefab/ground check.
            // You might want to adjust Y based on the prefab or a fixed lane.
            enemy.transform.position = new Vector3(data.xPosition, 0f, 0f);
            enemy.SetActive(true);
            activeEnemies.Add(enemy);
        }
    }

    GameObject GetFromPool(int type)
    {
        if (!enemyPools.ContainsKey(type))
        {
            enemyPools[type] = new Queue<GameObject>();
        }
        Queue<GameObject> pool = enemyPools[type];
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            // Pool empty, create new (expand pool)
            return CreateEnemy(type);
        }
    }

    void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        // We need to know which pool it belongs to.
        // Using a helper component 'EnemyIdentity' to store the type.
        EnemyIdentity id = enemy.GetComponent<EnemyIdentity>();
        if (id != null)
        {
            if (!enemyPools.ContainsKey(id.typeIndex))
                enemyPools[id.typeIndex] = new Queue<GameObject>();
            enemyPools[id.typeIndex].Enqueue(enemy);
        }
        else
        {
            Destroy(enemy); // If tracking is lost, just destroy to be safe
        }
    }
}

// Helper component to track pool origin
public class EnemyIdentity : MonoBehaviour
{
    public int typeIndex;
}
