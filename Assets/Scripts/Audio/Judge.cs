using UnityEngine;

public class Judge : MonoBehaviour
{
    public static Judge Instance { get; private set; }

    void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Found more than one Judge instance in the scene");
        }
        Instance = this;
    }

    /*
    void OnEnable()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.BeatEvent += HandleBeat;
        }
    }
    */

    void Start()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.BeatEvent += HandleBeat;
        }
    }

    void Update() { }

    /*
    void OnDisable()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.BeatEvent -= HandleBeat;
        }
    }
    */

    void OnDestroy()
    {
        if (Metronome.Instance != null)
            Metronome.Instance.BeatEvent -= HandleBeat;
    }

    private void HandleBeat(int beatIndex)
    {
        Debug.Log($"Judge received beat: {beatIndex}");
    }
}
