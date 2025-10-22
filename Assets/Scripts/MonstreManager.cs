using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [Header("Réglages")]
    public List<GameObject> monsterPrefabs; 
    public float yOffset = 1f;
    public Transform monsterParent;

    private List<GroundMonsterConfig> groundMonsterConfigs = new List<GroundMonsterConfig>();

    void Awake()
    {
        LoadMonsterLevel("levelMonster"); 
    }

    void LoadMonsterLevel(string fileName)
    {

        TextAsset textAsset = Resources.Load<TextAsset>("LevelData/" + fileName);
        if (textAsset == null)
        {
            Debug.LogError("❌ Fichier niveau monstres introuvable : " + fileName);
            return;
        }

        string[] entries = textAsset.text.Split(';');
        foreach (string entry in entries)
        {
            string[] parts = entry.Split(',');
            if (parts.Length != 3) continue;

            int groundIndex = int.Parse(parts[0]);
            float posPercent = float.Parse(parts[1]);
            int prefabIndex = int.Parse(parts[2]);

            GroundMonsterConfig config = groundMonsterConfigs.Find(c => c.groundIndex == groundIndex);
            if (config == null)
            {
                config = new GroundMonsterConfig { groundIndex = groundIndex, spawnPoints = new List<MonsterSpawnPoint>() };
                groundMonsterConfigs.Add(config);
            }

            config.spawnPoints.Add(new MonsterSpawnPoint { positionPercent = posPercent, monsterPrefabIndex = prefabIndex });
        }

        Debug.Log($"✅ Total configs chargées : {groundMonsterConfigs.Count}");
    }

    public void TrySpawnMonsterOnGround(GameObject ground, int groundLayoutIndex)
    {
        GroundMonsterConfig config = groundMonsterConfigs.Find(c => c.groundIndex == groundLayoutIndex);
        if (config == null)
        {
            return;
        }

        Renderer rend = ground.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("⚠️ Ground sans Renderer trouvé pour placement monstre");
            return;
        }

        float groundWidth = rend.bounds.size.x;
        Vector3 groundPos = ground.transform.position;

        foreach (var spawn in config.spawnPoints)
        {
            if (spawn.monsterPrefabIndex < 0 || spawn.monsterPrefabIndex >= monsterPrefabs.Count)
            {
                Debug.LogWarning($"⚠️ Index de prefab invalide : {spawn.monsterPrefabIndex}");
                continue;
            }

            GameObject prefab = monsterPrefabs[spawn.monsterPrefabIndex];

            float spawnX = groundPos.x - groundWidth / 2f + groundWidth * spawn.positionPercent;
            Vector3 spawnPos = new Vector3(spawnX, groundPos.y + yOffset, 0f);

            GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity, monsterParent ?? ground.transform);

            if (monster.GetComponent<MonsterIdle>() == null)
            {
                monster.AddComponent<MonsterIdle>();
            }

            if (!monster.GetComponent<Collider2D>()) monster.AddComponent<BoxCollider2D>();
            if (!monster.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = monster.AddComponent<Rigidbody2D>();
                rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            }
        }
    }
}

// Classes utilitaires
[System.Serializable]
public class MonsterSpawnPoint
{
    public float positionPercent;
    public int monsterPrefabIndex;
}

[System.Serializable]
public class GroundMonsterConfig
{
    public int groundIndex;
    public List<MonsterSpawnPoint> spawnPoints;
}
