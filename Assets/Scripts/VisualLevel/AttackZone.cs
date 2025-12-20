using UnityEngine;

public class AttackZone : MonoBehaviour
{
    public Color attackColor;

    // public delegate void HitRedBaseSamouraiEvent(RedBaseSamourai redBaseSamourai, float hitPostion);
    // public static event HitRedBaseSamouraiEvent HitRedBaseSamourai;

    // public delegate void HitGreenBaseSamouraiEvent(
    //     GreenBaseSamourai greenBaseSamourai,
    //     float hitPosition
    // );
    // public static event HitGreenBaseSamouraiEvent HitGreenBaseSamourai;
    //
    // public delegate void HitBlueBaseSamouraiEvent(
    //     BlueBaseSamourai blueBaseSamourai,
    //     float hitPosition
    // );
    // public static event HitBlueBaseSamouraiEvent HitBlueBaseSamourai;

    public delegate void HitRedBaseSamouraiEvent(RedBaseSamourai redBaseSamourai);
    public static event HitRedBaseSamouraiEvent HitRedBaseSamourai;

    public delegate void HitGreenBaseSamouraiEvent(GreenBaseSamourai greenBaseSamourai);
    public static event HitGreenBaseSamouraiEvent HitGreenBaseSamourai;

    public delegate void HitBlueBaseSamouraiEvent(BlueBaseSamourai blueBaseSamourai);
    public static event HitBlueBaseSamouraiEvent HitBlueBaseSamourai;

    public delegate void HitRedBaseBirdEvent(RedBaseBird redBaseBird);
    public static event HitRedBaseBirdEvent HitRedBaseBird;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackColor == Color.red)
        {
            RedBaseSamourai enemy = collision.GetComponent<RedBaseSamourai>();
            RedBaseBird bird = collision.GetComponent<RedBaseBird>();
            if (enemy != null)
            {
                // HitRedBaseSamourai?.Invoke(enemy, SoundPlayer.Instance.GetMusicPosition());
                HitRedBaseSamourai?.Invoke(enemy);
                // enemy.Die();
            }
            if (bird != null)
            {
                HitRedBaseBird?.Invoke(bird);
            }
        }
        if (attackColor == Color.green)
        {
            GreenBaseSamourai enemy = collision.GetComponent<GreenBaseSamourai>();
            if (enemy != null)
            {
                // HitGreenBaseSamourai?.Invoke(enemy, SoundPlayer.Instance.GetMusicPosition());
                HitGreenBaseSamourai?.Invoke(enemy);
                // enemy.Die();
            }
        }
        if (attackColor == Color.blue)
        {
            BlueBaseSamourai enemy = collision.GetComponent<BlueBaseSamourai>();
            if (enemy != null)
            {
                // HitBlueBaseSamourai?.Invoke(enemy, SoundPlayer.Instance.GetMusicPosition());
                HitBlueBaseSamourai?.Invoke(enemy);
                // enemy.Die();
            }
        }
    }
}
