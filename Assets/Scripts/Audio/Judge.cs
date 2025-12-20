using UnityEngine;

public class Judge : MonoBehaviour
{
    public static Judge Instance { get; private set; }

    private bool windowOpened = false;

    private int currentBeat = 0;

    public delegate void EnemyKilledEvent(int enemyBeat);
    public event EnemyKilledEvent EnemyKilled;

    // TO REMOVE if not needed for score calculation
    // public delegate void EnemyNotKilledEvent();
    // public event EnemyNotKilledEvent EnemyNotKilled;

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
        AttackZone.HitRedBaseSamourai += OnHitRedSamourai;
        AttackZone.HitGreenBaseSamourai += OnHitGreenSamourai;
        AttackZone.HitBlueBaseSamourai += OnHitBlueSamourai;
        AttackZone.HitRedBaseBird += OnHitRedBird;
    }

    void Update() { }

    void OnDisable()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.OpenWindow -= OpenWindow;
            Metronome.Instance.CloseWindow -= CloseWindow;
        }

        AttackZone.HitRedBaseSamourai -= OnHitRedSamourai;
        AttackZone.HitGreenBaseSamourai -= OnHitGreenSamourai;
        AttackZone.HitBlueBaseSamourai -= OnHitBlueSamourai;
        AttackZone.HitRedBaseBird -= OnHitRedBird;
    }

    void OnDestroy()
    {
        if (Metronome.Instance != null)
        {
            Metronome.Instance.OpenWindow -= OpenWindow;
            Metronome.Instance.CloseWindow -= CloseWindow;
        }

        AttackZone.HitRedBaseSamourai -= OnHitRedSamourai;
        AttackZone.HitGreenBaseSamourai -= OnHitGreenSamourai;
        AttackZone.HitBlueBaseSamourai -= OnHitBlueSamourai;
        AttackZone.HitRedBaseBird -= OnHitRedBird;
    }

    private void OpenWindow(int currentBeat)
    {
        // Debug.Log($"Judge: window {beatIndex} opened");
        windowOpened = true;
        this.currentBeat = currentBeat;
    }

    private void CloseWindow(int currentBeat)
    {
        // Debug.Log($"Judge: window {beatIndex} closed");
        windowOpened = false;
    }

    // private void OnHitRedSamourai(RedBaseSamourai enemy, float hitPosition)
    private void OnHitRedSamourai(RedBaseSamourai enemy)
    {
        if (windowOpened)
        {
            enemy.Die();
            EnemyKilled?.Invoke(this.currentBeat);
        }
    }

    // private void OnHitGreenSamourai(GreenBaseSamourai enemy, float hitPosition)
    private void OnHitGreenSamourai(GreenBaseSamourai enemy)
    {
        if (windowOpened)
        {
            enemy.Die();
            EnemyKilled?.Invoke(this.currentBeat);
        }
    }

    // private void OnHitBlueSamourai(BlueBaseSamourai enemy, float hitPosition)
    private void OnHitBlueSamourai(BlueBaseSamourai enemy)
    {
        if (windowOpened)
        {
            enemy.Die();
            EnemyKilled?.Invoke(this.currentBeat);
        }
    }

    private void OnHitRedBird(RedBaseBird enemy)
    {
        if (windowOpened)
        {
            enemy.Die();
            EnemyKilled?.Invoke(this.currentBeat);
        }
    }

    private void OnHitGreenBird(RedBaseBird enemy)
    {
        if (windowOpened)
        {
            enemy.Die();
            EnemyKilled?.Invoke(this.currentBeat);
        }
    }

    private void OnHitBlueBird(RedBaseBird enemy)
    {
        if (windowOpened)
        {
            enemy.Die();
            EnemyKilled?.Invoke(this.currentBeat);
        }
    }
}
