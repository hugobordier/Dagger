using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles loading and spawning of monsters in the level.
/// Monsters are configured via a level data file located in Resources/LevelData.
/// Each entry defines which monster prefab to spawn on a specific ground at a given position.
/// </summary>
public class MonsterManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("List of available monster prefabs.")]
    public List<GameObject> monsterPrefabs;

    [Tooltip("Vertical offset from the ground when spawning monsters.")]
    public float yOffset = 1f;

    [Tooltip("Optional parent object for all spawned monsters.")]
    public Transform monsterParent;

    // Internal list holding the monster configurations for each ground segment
    private readonly List<GroundMonsterConfig> groundMonsterConfigs = new List<GroundMonsterConfig>();

    // =====================================================================
    // Unity Lifecycle
    // =====================================================================

    private void Awake()
    {
        LoadMonsterLevel("levelMonster");
    }

    // =====================================================================
    // Level Data Loading
    // =====================================================================

    /// <summary>
    /// Loads monster configuration data from a text file located in Resources/LevelData.
    /// Each line defines one or more monsters to spawn, formatted as:
    ///     groundIndex,positionPercent,prefabIndex;
    /// Example:
    ///     0,0.25,1;0,0.75,0;1,0.5,2;
    /// </summary>
    /// <param name="fileName">The name of the file (without extension).</param>
    private void LoadMonsterLevel(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("LevelData/" + fileName);
        Debug.Log("Loading monster level file: " + textAsset);

        if (textAsset == null)
        {
            Debug.LogError("Monster level file not found: " + fileName);
            return;
        }

        // Parse the file into structured spawn data
        string[] entries = textAsset.text.Split(';');
        foreach (string entry in entries)
        {
            string[] parts = entry.Split(',');
            if (parts.Length != 3) continue;

            int groundIndex = int.Parse(parts[0]);
            float posPercent = float.Parse(parts[1]);
            int prefabIndex = int.Parse(parts[2]);

            // Find or create configuration for this ground
            GroundMonsterConfig config = groundMonsterConfigs.Find(c => c.groundIndex == groundIndex);
            if (config == null)
            {
                config = new GroundMonsterConfig
                {
                    groundIndex = groundIndex,
                    spawnPoints = new List<MonsterSpawnPoint>()
                };
                groundMonsterConfigs.Add(config);
            }

            // Add the monster spawn point
            config.spawnPoints.Add(new MonsterSpawnPoint
            {
                positionPercent = posPercent,
                monsterPrefabIndex = prefabIndex
            });
        }

        Debug.Log($"Monster configs loaded: {groundMonsterConfigs.Count}");
    }

    // =====================================================================
    // Monster Spawning
    // =====================================================================

    /// <summary>
    /// Spawns all configured monsters for a specific ground object.
    /// </summary>
    /// <param name="ground">The ground GameObject where monsters should spawn.</param>
    /// <param name="groundLayoutIndex">The index of the ground (as defined in the level file).</param>
    public void TrySpawnMonsterOnGround(GameObject ground, int groundLayoutIndex)
    {
        GroundMonsterConfig config = groundMonsterConfigs.Find(c => c.groundIndex == groundLayoutIndex);
        if (config == null) return;

        Renderer rend = ground.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("Ground object has no Renderer, cannot determine width for monster placement.");
            return;
        }

        float groundWidth = rend.bounds.size.x;
        Vector3 groundPos = ground.transform.position;

        foreach (var spawn in config.spawnPoints)
        {
            // Ensure prefab index is valid
            if (spawn.monsterPrefabIndex < 0 || spawn.monsterPrefabIndex >= monsterPrefabs.Count)
            {
                Debug.LogWarning($"Invalid prefab index: {spawn.monsterPrefabIndex}");
                continue;
            }

            GameObject prefab = monsterPrefabs[spawn.monsterPrefabIndex];

            // Compute world position of the monster on the ground
            float spawnX = groundPos.x - groundWidth / 2f + groundWidth * spawn.positionPercent;
            Vector3 spawnPos = new Vector3(spawnX, groundPos.y + yOffset, 0f);

            // Instantiate the monster
            GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity, monsterParent ?? ground.transform);

            // Ensure the monster has an idle behavior
            if (monster.GetComponent<MonsterIdle>() == null)
                monster.AddComponent<MonsterIdle>();

            // Ensure collider is a trigger (so the player can pass through)
            Collider2D col = monster.GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            // Add Rigidbody2D if missing, with restricted motion
            if (!monster.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = monster.AddComponent<Rigidbody2D>();
                rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            }

            // Add hit detection component if not already present
            if (!monster.GetComponent<MonsterHitLogger>())
                monster.AddComponent<MonsterHitLogger>();
        }
    }
}

// =====================================================================
// Helper Data Classes
// =====================================================================

/// <summary>
/// Defines where and which monster prefab should be spawned on a given ground.
/// </summary>
[System.Serializable]
public class MonsterSpawnPoint
{
    public float positionPercent;
    public int monsterPrefabIndex;
}

/// <summary>
/// Configuration for all monster spawn points on a specific ground.
/// </summary>
[System.Serializable]
public class GroundMonsterConfig
{
    public int groundIndex;
    public List<MonsterSpawnPoint> spawnPoints;
}
