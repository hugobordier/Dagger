using UnityEngine;

public class CameraManger : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform m_camera;
    [SerializeField] private GroundManager groundManager;
    [SerializeField] private float heightStep = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void LateUpdate()
    {
        if (m_camera != null && groundManager != null)
        {
            Vector3 camPos = m_camera.position;
            camPos.x = player.position.x + 5.0f;

            float heightOffset = 1.74f + groundManager.currentHeightOffset * heightStep;
            // Debug.Log($"Cam offset reçu = {groundManager.currentHeightOffset}");
            camPos.y = Mathf.Lerp(camPos.y, heightOffset, Time.deltaTime * 6f);;

            m_camera.position = camPos;
        }
    }
}
