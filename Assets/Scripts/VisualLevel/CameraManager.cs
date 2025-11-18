using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform m_camera;
    [SerializeField] private GroundManager groundManager;
    [SerializeField] private float heightStep = 3f;
    private Player playerScript;
    private float yFollowDelay;      // durée avant de suivre
    private float yFollowTimer = 0f; // compteur du timer
    private bool canFollowY = true;  // état de la caméra
    private float lastHeightOffset = 0f; // pour détecter les changements

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player != null)
            playerScript = player.GetComponent<Player>();
        else
            Debug.LogError("Player transform n'est pas assigné dans CameraManager.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (m_camera == null || groundManager == null || player == null || playerScript == null)
            return;

        Vector3 camPos = m_camera.position;
        camPos.x = player.position.x + 5.0f;

        if (groundManager.currentHeightOffset != lastHeightOffset)
        {
            yFollowDelay = 60.0f / Mathf.Max(0.01f, playerScript.m_TranslationSpeed); // éviter division par zéro
            yFollowTimer = yFollowDelay;
            canFollowY = false;

            lastHeightOffset = groundManager.currentHeightOffset;
            Debug.Log($"💡 Nouvelle hauteur détectée ! Délai de {yFollowDelay:F2}s avant suivi Y.");
        }

        if (!canFollowY)
        {
            yFollowTimer -= Time.deltaTime;
            if (yFollowTimer <= 0f)
            {
                canFollowY = true;
                Debug.Log("⏱️ La caméra reprend le suivi Y !");
            }
        }

        if (canFollowY)
        {
            float targetY = 1.74f + groundManager.currentHeightOffset * heightStep;
            camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * 1f);
        }

        m_camera.position = camPos;
    }
}
