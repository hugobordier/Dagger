using System.Threading;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private EventInstance musicInstance;
    public bool IsMusicPlaying { get; private set; }

    public static SoundPlayer Instance { get; private set; }

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
        // AudioManager.Instance.PlayOneShot(FMODEvents.Instance.MusicLevel1);
        IsMusicPlaying = false;
        musicInstance = RuntimeManager.CreateInstance(FMODEvents.Instance.MusicLevel1);
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
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
