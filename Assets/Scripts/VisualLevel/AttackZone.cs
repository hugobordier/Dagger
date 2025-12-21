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

    public delegate void HitRedBaseSamouraiEvent(RedBaseSamourai samourai);
    public static event HitRedBaseSamouraiEvent HitRedBaseSamourai;

    public delegate void HitGreenBaseSamouraiEvent(GreenBaseSamourai samourai);
    public static event HitGreenBaseSamouraiEvent HitGreenBaseSamourai;

    public delegate void HitBlueBaseSamouraiEvent(BlueBaseSamourai samourai);
    public static event HitBlueBaseSamouraiEvent HitBlueBaseSamourai;

    public delegate void HitRedBaseBirdEvent(RedBaseBird bird);
    public static event HitRedBaseBirdEvent HitRedBaseBird;

    public delegate void HitGreenBaseBirdEvent(GreenBaseBird bird);
    public static event HitGreenBaseBirdEvent HitGreenBaseBird;

    public delegate void HitBlueBaseBirdEvent(BlueBaseBird bird);
    public static event HitBlueBaseBirdEvent HitBlueBaseBird;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackColor == Color.red)
        {
            RedBaseSamourai enemy = collision.GetComponent<RedBaseSamourai>();
            RedBaseBird bird = collision.GetComponent<RedBaseBird>();
            if (enemy != null)
            {
                HitRedBaseSamourai?.Invoke(enemy);
            }
            if (bird != null)
            {
                HitRedBaseBird?.Invoke(bird);
            }
        }
        if (attackColor == Color.green)
        {
            GreenBaseSamourai enemy = collision.GetComponent<GreenBaseSamourai>();
            GreenBaseBird bird = collision.GetComponent<GreenBaseBird>();
            if (enemy != null)
            {
                HitGreenBaseSamourai?.Invoke(enemy);
            }
            if (bird != null)
            {
                HitGreenBaseBird?.Invoke(bird);
            }
        }
        if (attackColor == Color.blue)
        {
            BlueBaseSamourai enemy = collision.GetComponent<BlueBaseSamourai>();
            BlueBaseBird bird = collision.GetComponent<BlueBaseBird>();
            if (enemy != null)
            {
                HitBlueBaseSamourai?.Invoke(enemy);
            }
            if (bird != null)
            {
                HitBlueBaseBird?.Invoke(bird);
            }
        }
    }
}
