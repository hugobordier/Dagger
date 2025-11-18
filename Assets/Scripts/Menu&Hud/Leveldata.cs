using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance;
    public string selectedLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre scènes
        }
        else Destroy(gameObject); // éviter les doublons
    }
}