using UnityEngine;
using UnityEngine.SceneManagement;



public class MenuManager : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] GameObject m_PanelMainMenu;
	[SerializeField] GameObject m_PanelPauseMenu;


	GameObject currentPanel;
	public string selectedLevel;

	void Start()
	{
		Debug.Log("MenuManager Start");
        OpenMainMenu();
    }

	// Update is called once per frame
	public void OpenMainMenu()
	{
		Debug.Log("OpenMainMenu");
		CloseAllPanels();
		m_PanelMainMenu.SetActive(true);
		currentPanel = m_PanelMainMenu;
	}

	public void OpenPauseMenu()
	{
		CloseAllPanels();
		m_PanelPauseMenu.SetActive(true);
		currentPanel = m_PanelPauseMenu;
		Time.timeScale = 0f; // Pause du jeu
	}
	
	public void ClosePauseMenu()
    {
        m_PanelPauseMenu.SetActive(false);
        currentPanel = null;
        Time.timeScale = 1f; // Reprise du jeu
    }

	void CloseAllPanels()
	{
		m_PanelMainMenu.SetActive(false);
		m_PanelPauseMenu.SetActive(false);
	}

	public void LoadLevel(string LevelName)
	{
		Debug.Log("LoadLevel: " + LevelName);
		LevelData.Instance.selectedLevel = LevelName;

		SceneManager.LoadScene("LevelScene", LoadSceneMode.Additive);
		m_PanelMainMenu.SetActive(false);
	}

	public void QuitGame()
	{
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
	
	public void QuitToMainMenu()
    {
		Time.timeScale = 1f;
		SceneManager.UnloadSceneAsync("LevelScene");
		OpenMainMenu();
    }
}
