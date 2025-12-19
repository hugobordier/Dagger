using System;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }
    private bool isRunning = false;

    [SerializeField]
    private float _bpm = 89;

    public float Bpm
    {
        get { return _bpm; }
    }

    [SerializeField]
    private float marginMs = 200;
    private float marginOffsetToBeatMs;

    private int currentBeat = 0;
    private float beatDurationMs;
    private float nextBeatPositionMs = 0;
    private float activeBeatStartPositionMs = 0;
    private float activeBeatEndPositionMs = 0;

    public delegate void OpenWindowEvent(int beat, float position);
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
        beatDurationMs = 60f / _bpm * 1000f;
        marginOffsetToBeatMs = marginMs / 2.0f;
    }

    void Update()
    {
        if (!isRunning || !SoundPlayer.Instance.IsMusicPlaying)
            return;
        int position = SoundPlayer.Instance.GetMusicPosition();
        if (position >= activeBeatStartPositionMs)
        {
            OpenWindow?.Invoke(currentBeat, nextBeatPositionMs);
            nextBeatPositionMs += beatDurationMs;
            activeBeatStartPositionMs = nextBeatPositionMs - marginOffsetToBeatMs;
            // Debug.Log($"Beat {currentBeat} opened at {position}");
        }
        if (position >= activeBeatEndPositionMs)
        {
            CloseWindow?.Invoke(currentBeat);
            activeBeatEndPositionMs = nextBeatPositionMs + marginOffsetToBeatMs;
            // Debug.Log($"Beat {currentBeat} closed at {position}");
            currentBeat += 1;
        }
    }

    public void StartMetronome()
    {
        isRunning = true;
        currentBeat = 0;
    }

    public void StopMetronome()
    {
        isRunning = false;
    }
}
