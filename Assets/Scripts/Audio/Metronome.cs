using UnityEngine;

public class Metronome : MonoBehaviour
{
    [SerializeField]
    private float bpm = 120f;

    private float secondsPerBeat;
    private float timer = 0f;
    private float nbBeats = 0f;
    private bool isRunning = false;

    void Start()
    {
        secondsPerBeat = 60f / bpm;
    }

    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;
        if (timer >= secondsPerBeat)
        {
            nbBeats += 1f;
            
            // Afficher la position de la musique tous les 4 temps
            if (nbBeats % 4 == 0)
            {
                int musicPosition = SoundPlayer.Instance.GetMusicPosition();
                float musicPositionSeconds = musicPosition / 1000f;
                Debug.Log($"Beat {nbBeats} | Position musique: {musicPositionSeconds:F2}s ({musicPosition}ms)");
            }
            
            timer -= secondsPerBeat;
        }
    }

    public void StartMetronome()
    {
        isRunning = true;
        timer = 0f;
        nbBeats = 0f;
    }

    public void StopMetronome()
    {
        isRunning = false;
    }
}