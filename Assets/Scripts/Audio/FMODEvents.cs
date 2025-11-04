using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    // [field: Header("Music Level 1")]
    // Music of level 1
    [field: SerializeField]
    public EventReference MusicLevel1 { get; private set; }

    // [field: Header("Music Level 2")]
    // Music of level 2
    [field: SerializeField]
    public EventReference MusicLevel2 { get; private set; }

    // FMOD Events
    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        Instance = this;
    }
}
