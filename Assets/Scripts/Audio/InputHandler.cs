using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Input Handler instance in the scene");
        }
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SoundPlayer.Instance.IsMusicPlaying == false)
        {
            SoundPlayer.Instance.PlayMusic();
        }
        if (Input.GetKeyDown(KeyCode.Backspace) && SoundPlayer.Instance.IsMusicPlaying == true)
        {
            SoundPlayer.Instance.StopMusic();
        }
    }
}
