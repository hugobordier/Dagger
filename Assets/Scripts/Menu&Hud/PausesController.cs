using UnityEngine;

public class PausesController : MonoBehaviour
{
    [SerializeField] MenuManager menuManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1f)
                menuManager.OpenPauseMenu();
            else
                menuManager.ClosePauseMenu();
        }
    }
}
