using System;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }
    private bool isRunning = false;

    [SerializeField]
    private float bpm = 89;

    [SerializeField]
    private float marginMs = 100;

    private int currentBeat = 0;
    private float beatDurationMs;
    private float nextBeatPositionMs = 0;
    private float activeBeatStartPositionMs = 0;
    private float activeBeatEndPositionMs = 0;

    public delegate void OpenWindowEvent(int beat);
    public event OpenWindowEvent OpenWindow;

    public delegate void CloseWindowEvent(int beat);
    public event CloseWindowEvent CloseWindow;

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
        if (position >= activeBeatStartPositionMs)
        {
            OpenWindow?.Invoke(currentBeat);
            nextBeatPositionMs += beatDurationMs;
            activeBeatStartPositionMs = nextBeatPositionMs - marginMs;
        }
        if (position >= activeBeatEndPositionMs)
        {
            CloseWindow?.Invoke(currentBeat);
            activeBeatEndPositionMs = nextBeatPositionMs + marginMs;
            currentBeat += 1;
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
