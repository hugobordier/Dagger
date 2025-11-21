using UnityEngine;

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

    public void StartGame()
    {
        if (SoundPlayer.Instance != null && Metronome.Instance != null)
        {
            SoundPlayer.Instance.PlayMusic();
            Metronome.Instance.StartMetronome();
            Debug.Log("Game started : components synchronized");
        }
        else
        {
            Debug.LogError("SoundPlayer or Metronome not found!");
        }
    }

    /// Arrête la musique et le métronome
    public void StopGame()
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
        StopGame();
    }
}
