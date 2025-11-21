using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    private EventInstance musicInstance;
    public bool IsMusicPlaying { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Sound Player instance in the scene");
        }
        Instance = this;
    }

    void Start()
    {
        IsMusicPlaying = false;
        musicInstance = RuntimeManager.CreateInstance(FMODEvents.Instance.MusicLevel1);
    }

    public void PlayMusic()
    {
        IsMusicPlaying = true;
        musicInstance.start();
    }

    public void StopMusic()
    {
        IsMusicPlaying = false;
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public int GetMusicPosition()
    {
        if (IsMusicPlaying)
        {
            musicInstance.getTimelinePosition(out int position);
            return position;
        }
        return 0;
    }

    private void OnDestroy()
    {
        if (IsMusicPlaying)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }
}
