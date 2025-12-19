using UnityEngine;

public class Judge : MonoBehaviour
{
    public static Judge Instance { get; private set; }

    private float position;
    private bool windowOpened = false;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Found more than one Judge instance in the scene");
        }
        Instance = this;
    }

    private static AttackZone[] attackZones;

    void Start()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.OpenWindow += OpenWindow;
            Metronome.Instance.CloseWindow += CloseWindow;
        }

        AttackZone.HitRedBaseSamourai += OnHitRed;
        AttackZone.HitGreenBaseSamourai += OnHitGreen;
        AttackZone.HitBlueBaseSamourai += OnHitBlue;

        position = 0;
    }

    void Update() { }

    void OnDisable()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.OpenWindow -= OpenWindow;
            Metronome.Instance.CloseWindow -= CloseWindow;
        }

        AttackZone.HitRedBaseSamourai -= OnHitRed;
        AttackZone.HitGreenBaseSamourai -= OnHitGreen;
        AttackZone.HitBlueBaseSamourai -= OnHitBlue;
    }

    void OnDestroy()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.OpenWindow -= OpenWindow;
            Metronome.Instance.CloseWindow -= CloseWindow;
        }

        AttackZone.HitRedBaseSamourai -= OnHitRed;
        AttackZone.HitGreenBaseSamourai -= OnHitGreen;
        AttackZone.HitBlueBaseSamourai -= OnHitBlue;
    }

    private void OpenWindow(int beatIndex, float position)
    {
        // Debug.Log($"Judge: window {beatIndex} opened");
        windowOpened = true;
        this.position = position;
    }

    private void CloseWindow(int beatIndex)
    {
        // Debug.Log($"Judge: window {beatIndex} closed");
        windowOpened = false;
    }

    private void OnHitRed(RedBaseSamourai enemy, float hitPosition)
    // private void OnHitRed(RedBaseSamourai enemy)
    {
        if (windowOpened)
        {
            // Debug.Log("Perfect Hit Red!");
            enemy.Die();
        }
        else
        {
            // Debug.Log($"Bad Timing Red! Off : {hitPosition - this.position} ms at {this.position}");
        }
    }

    // private void OnHitGreen(GreenBaseSamourai enemy, float hitPosition)
    private void OnHitGreen(GreenBaseSamourai enemy)
    {
        if (windowOpened)
        {
            // Debug.Log("Perfect Hit Green!");
            enemy.Die();
        }
        else
        {
            // Debug.Log(
            //     $"Bad Timing Green! Off : {hitPosition - this.position} ms at {this.position}"
            // );
        }
    }

    // private void OnHitBlue(BlueBaseSamourai enemy, float hitPosition)
    private void OnHitBlue(BlueBaseSamourai enemy)
    {
        if (windowOpened)
        {
            // Debug.Log("Perfect Hit Blue!");
            enemy.Die();
        }
        else
        {
            // Debug.Log(
            //     $"Bad Timing Blue! Off : {hitPosition - this.position} ms at {this.position}"
            // );
        }
    }
}
