using System;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }

    [SerializeField]
    private float bpm = 89f;

    private float beatDurationMs;
    private int currentBeat = 0;
    private float nextBeatPositionMs = 0f;
    private bool isRunning = false;

    public delegate void CurrentBeatEvent(int beat);
    public event CurrentBeatEvent BeatEvent;

    void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Found more than one Metronome instance in the scene");
        }
        Instance = this;
    }

    void Start()
    {
        beatDurationMs = 60f / bpm * 1000f;
    }

    void Update()
    {
        if (!isRunning || !SoundPlayer.Instance.IsMusicPlaying)
            return;
        int position = SoundPlayer.Instance.GetMusicPosition();
        if (position >= nextBeatPositionMs)
        {
            currentBeat += 1;
            nextBeatPositionMs += beatDurationMs;
            BeatEvent?.Invoke(currentBeat);
        }
    }

    public void StartMetronome()
    {
        isRunning = true;
        currentBeat = -1;
    }

    public void StopMetronome()
    {
        isRunning = false;
    }
}
