using UnityEngine;

public class RedBaseSamourai : MonoBehaviour
{
    private Player player;
    private bool alive = false;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Player.RedAttack += RedAttackAction;
    }

    // Update is called once per frame
    void Update() { }

    void RedAttackAction()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }
}
