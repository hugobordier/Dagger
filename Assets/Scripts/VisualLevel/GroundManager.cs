using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundManager : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private GameObject rythmPrefab; // prefab pour le rythme
    [SerializeField] private GroundLibrary groundLibrary; // prefab library
    [SerializeField] private MonsterManager monsterManager;

    public float groundWidth = 10f; // largeur du prefab
    public float baseHeight = -1.74f; // hauteur de base pour le ground
    public float heightStep = 3f; // hauteur ajouter pour chaque Ground UP 
    private List<GameObject> rythmgrounds = new List<GameObject>(); // liste des prefabs de rythm infini
    private List<GameObject> grounds = new List<GameObject>(); // liste des prefabs de ground
    private Dictionary<int, GameObject> groundPrefabs = new(); // dictionnaire des prefabs de Ground
    private List<int> levelLayout = new List<int>(); // liste des indices de prefabs pour le niveau
    private int currentGroundIndex = 0;
    public float currentHeightOffset = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Charger les prefabs depuis la librairie
        for (int i = 0; i < groundLibrary.groundPrefabs.Length; i++)
            groundPrefabs[i] = groundLibrary.groundPrefabs[i];

        // Initialiser le layout du niveau
        
        // Hugo si tu veux travailler sur LevelScene sans te casser la tete, 
        // commente les 2 lignes en dessous
        // string levelName = LevelData.Instance.selectedLevel;
        // LoadLevelLayout(levelName);

        // et décommente cet ligne, en ayant la scene LevelScene chargé dans unity
        // si c'est pas bon dis le moi.
        LoadLevelLayout("leveltest");


        // Initialiser les grounds initiaux
        for (int i = -1; i <= 2; i++)
        {
            SpawnNextGround(i, i);
            currentGroundIndex++;
        }
    }

    public void LoadLevelLayout(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("LevelData/" + fileName);

        if (textAsset != null)
        {
            string[] numbers = textAsset.text.Split(';');
            foreach (string n in numbers)
            {
                if (int.TryParse(n, out int index))
                    levelLayout.Add(index);
            }
        }
        else
        {
            Debug.LogError("Fichier non trouvé : " + fileName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float playerX = player.position.x;
        GameObject leftMost = grounds[0];
        GameObject leftMostrythm = rythmgrounds[0];
        GameObject rightMost = grounds[grounds.Count - 1];

        if (playerX > rightMost.transform.position.x - groundWidth)
        {
            // Spawn nouveau à droite
            currentGroundIndex++;
            Debug.Log("Spawning new ground at index: " + currentGroundIndex);
            SpawnNextGround(currentGroundIndex, rightMost.transform.position.x / groundWidth + 1);
            // Supprime celui de gauche
            Destroy(leftMost);
            grounds.RemoveAt(0);
            Destroy(leftMostrythm);
            rythmgrounds.RemoveAt(0);
        }

        // if (playerX < leftMost.transform.position.x + groundWidth / 2)
        // {
        //     GameObject g = Instantiate(rythmPrefab, leftMost.transform.position - Vector3.right * groundWidth, Quaternion.identity);
        //     grounds.Insert(0, g);
        // 
        //     Destroy(rightMost);
        //     grounds.RemoveAt(grounds.Count - 1);
        // }
    }

    void SpawnNextGround(int layoutIndex, float positionIndex)
    {
        if (layoutIndex < 0 || layoutIndex >= levelLayout.Count) return;

        int prefabIndex = levelLayout[layoutIndex];

        //        if (prefabIndex >= 5) // UP
        //            currentHeightOffset += 1f;

        float yPos = baseHeight + (currentHeightOffset * heightStep);
        Vector3 position = new Vector3(positionIndex * groundWidth, yPos, 0);


        if (!groundPrefabs.TryGetValue(prefabIndex, out GameObject prefab))
        {
            prefab = rythmPrefab;
        }

        GameObject g = Instantiate(prefab, position, Quaternion.identity);
        grounds.Add(g);

        Debug.Log($"Spawn Ground index {prefabIndex} à la position {position}");

        monsterManager?.TrySpawnMonsterOnGround(g, (int)position.x / 10); // c'est pas super propre mais ça marche 
        //monsterManager?.TrySpawnMonsterOnGround(g, layoutIndex);

        GameObject rythm = Instantiate(rythmPrefab, position, Quaternion.identity);
        rythmgrounds.Add(rythm);

        Scene levelScene = SceneManager.GetSceneByName("LevelScene");
        if (levelScene.IsValid())
            SceneManager.MoveGameObjectToScene(g, levelScene);
        
        if (prefabIndex >= 5) // UP
        {
            currentHeightOffset += 1f;
        }

        // Spawn rythm ground, pas à garder juste pour le visuel
        float yPos2 = baseHeight + (currentHeightOffset * heightStep);
        Vector3 position2 = new Vector3(positionIndex * groundWidth, yPos2, 0);

        rythm = Instantiate(rythmPrefab, position2, Quaternion.identity);
        rythmgrounds.Add(rythm);

        if (levelScene.IsValid())
            SceneManager.MoveGameObjectToScene(rythm, levelScene);
    }

}
