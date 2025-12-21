using UnityEngine;

[DefaultExecutionOrder(-20)]
public class Referee : MonoBehaviour
{
    public static Referee Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Referee instance in the scene");
        }
        Instance = this;
    }

    void Start() { }

    void Update()
    {
        if (SoundPlayer.Instance != null && SoundPlayer.Instance.IsMusicPlaying)
        {
            if (SoundPlayer.Instance.GetPlaybackState() == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                OnMusicFinished();
            }
        }
    }

    private void OnMusicFinished()
    {
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenEndMenu();
        }
        else
        {
            Debug.LogError("MenuManager instance not found!");
        }
        StopAudioPipeline();
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.Stop();
        }
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.Stop();
        }
    }

    public void StartAudioPipeline()
    {
        if (SoundPlayer.Instance != null && Metronome.Instance != null)
        {
            SoundPlayer.Instance.PlayMusic();
            Metronome.Instance.StartMetronome();
        }
        else
        {
            Debug.LogError("SoundPlayer or Metronome not found!");
        }
    }

    public void StopAudioPipeline()
    {
        if (SoundPlayer.Instance != null && Metronome.Instance != null)
        {
            SoundPlayer.Instance.StopMusic();
            Metronome.Instance.StopMetronome();
        }
    }

    public void PauseAudioPipeline()
    {
        if (SoundPlayer.Instance != null && Metronome.Instance != null)
        {
            SoundPlayer.Instance.PauseMusic();
            Metronome.Instance.StopMetronome();
        }
    }

    public void ResumeAudioPipeline()
    {
        if (SoundPlayer.Instance != null && Metronome.Instance != null)
        {
            SoundPlayer.Instance.ResumeMusic();
            Metronome.Instance.ResumeMetronome();
        }
    }

    /// Pause le jeu
    public void PauseGame()
    {
        PauseAudioPipeline();
    }
}
