using UnityEngine;

public class Metronome : MonoBehaviour
{
    [SerializeField]
    private float bpm = 120f;

    private float secondsPerBeat;
    private float timer = 0f;
    private float nbBeats = 0f;

    void Start()
    {
        secondsPerBeat = 60f / bpm;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerBeat)
        {
            nbBeats += 1f;
            Debug.Log(nbBeats);
            timer -= secondsPerBeat;
        }
    }
}