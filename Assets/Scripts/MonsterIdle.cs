using UnityEngine;

public class MonsterIdle : MonoBehaviour
{
    [Header("Réglages du saut")]
    public float jumpHeight = 0.5f; 
    public float jumpSpeed = 2f;    

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * jumpSpeed, 1f);

        float yOffset = 4f * jumpHeight * t * (1 - t); 

        transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
    }
}
