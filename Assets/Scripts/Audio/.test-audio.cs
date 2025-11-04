using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Header("FMOD Settings")]
    [SerializeField]
    private string bankName = "Music";

    [SerializeField]
    private string eventPath = "event:/Music/MusicLevel1";

    public static SoundPlayer Instance { get; private set; }
    private EventInstance musicInstance;
    private bool isMusicPlaying = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one SoundPlayer in the scene");
        }
        Instance = this;
    }

    void Start()
    {
        LoadBank();
        musicInstance = RuntimeManager.CreateInstance(eventPath);
        musicInstance.start();
        isMusicPlaying = true;
        Debug.Log($"Playing FMOD music : {eventPath}");
    }

    private void LoadBank()
    {
        try
        {
            RuntimeManager.LoadBank(bankName, true);
            Debug.Log($"FMOD bank '{bankName}' loaded successfully.");
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error loading FMOD bank '{bankName}': {e.Message}");
        }
    }

    private void OnDestroy()
    {
        if (isMusicPlaying)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }
}
