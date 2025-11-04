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
            if (nbBeats % 4 == 0)
            {
                Debug.Log(nbBeats);
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