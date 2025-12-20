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
        // Debug.Log("Music Finished! Triggering Level End.");
        // 1. Launch PanelEndMenu in MainMenuScene
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenEndMenu();
        }
        else
        {
            Debug.LogError("MenuManager instance not found!");
        }

        // 2. Stop AudioManager (SoundPlayer/Referee)
        StopAudioPipeline();

        // 3. Stop EnemyManager
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.Stop();
        }

        // 4. Stop Player
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

    /// Arrête la musique et le métronome
    public void StopAudioPipeline()
    {
        if (SoundPlayer.Instance != null && Metronome.Instance != null)
        {
            SoundPlayer.Instance.StopMusic();
            Metronome.Instance.StopMetronome();
            Debug.Log("Game stopped");
        }
    }

    /// Pause le jeu (TODO)
    public void PauseGame()
    {
        StopAudioPipeline();
    }
}
