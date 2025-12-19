using UnityEngine;

public class AttackZone : MonoBehaviour
{
    public Color attackColor;

    public delegate void HitRedBaseSamouraiEvent(RedBaseSamourai redBaseSamourai, float hitPostion);
    public static event HitRedBaseSamouraiEvent HitRedBaseSamourai;

    public delegate void HitGreenBaseSamouraiEvent(
        GreenBaseSamourai greenBaseSamourai,
        float hitPosition
    );
    public static event HitGreenBaseSamouraiEvent HitGreenBaseSamourai;

    public delegate void HitBlueBaseSamouraiEvent(
        BlueBaseSamourai blueBaseSamourai,
        float hitPosition
    );
    public static event HitBlueBaseSamouraiEvent HitBlueBaseSamourai;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackColor == Color.red)
        {
            RedBaseSamourai enemy = collision.GetComponent<RedBaseSamourai>();
            if (enemy != null)
            {
                HitRedBaseSamourai?.Invoke(enemy, SoundPlayer.Instance.GetMusicPosition());
                // enemy.Die();
            }
        }
        if (attackColor == Color.green)
        {
            GreenBaseSamourai enemy = collision.GetComponent<GreenBaseSamourai>();
            if (enemy != null)
            {
                HitGreenBaseSamourai?.Invoke(enemy, SoundPlayer.Instance.GetMusicPosition());
                // enemy.Die();
            }
        }
        if (attackColor == Color.blue)
        {
            BlueBaseSamourai enemy = collision.GetComponent<BlueBaseSamourai>();
            if (enemy != null)
            {
                HitBlueBaseSamourai?.Invoke(enemy, SoundPlayer.Instance.GetMusicPosition());
                // enemy.Die();
            }
        }
    }
}
