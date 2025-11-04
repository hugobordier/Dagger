using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField]
    private SoundPlayer soundPlayer;
    
    [SerializeField]
    private Metronome metronome;

    public static Referee Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Referee instance in the scene");
        }
        Instance = this;
    }

    void Start()
    {
        // Chercher automatiquement les composants s'ils ne sont pas assignés
        if (soundPlayer == null)
        {
            soundPlayer = FindFirstObjectByType<SoundPlayer>();
        }
        
        if (metronome == null)
        {
            metronome = FindFirstObjectByType<Metronome>();
        }
    }

    /// synchronise tout ça
    public void StartGame()
    {
        if (soundPlayer != null && metronome != null)
        {
            // Démarrer les deux au même moment
            soundPlayer.PlayMusic();
            metronome.StartMetronome();

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
        if (soundPlayer != null && metronome != null)
        {
            soundPlayer.StopMusic();
            metronome.StopMetronome();
            
            Debug.Log("Game stopped");
        }
    }

    /// Pause le jeu (TODO)
    public void PauseGame()
    {
        StopGame();
    }
}
