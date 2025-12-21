using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI m_ScoreText;

    [Header("Panels")]
    [SerializeField]
    GameObject m_PanelMainMenu;

    [SerializeField]
    GameObject m_PanelPauseMenu;

    [SerializeField]
    GameObject m_PanelGameoverMenu;

    [SerializeField]
    GameObject m_PanelEndMenu;

    [SerializeField]
    GameObject m_PanelCommandMenu;
    public bool IsEasyMode { get; private set; } = false;
    GameObject currentPanel;
    private string selectedLevel;

    void Start()
    {
        Debug.Log("MenuManager Start");
        OpenMainMenu();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (Referee.Instance != null) Referee.Instance.PauseGame();
        Time.timeScale = 0f; // Pause du jeu
    }

    public void ClosePauseMenu()
    {
        m_PanelPauseMenu.SetActive(false);
        currentPanel = null;
        if (Referee.Instance != null) Referee.Instance.ResumeAudioPipeline();
        Time.timeScale = 1f; // Reprise du jeu
    }

    public void OpenGameoverMenu()
    {
        if (EnemyManager.Instance != null) EnemyManager.Instance.Stop();
        m_PanelGameoverMenu.SetActive(true);
        currentPanel = m_PanelGameoverMenu;
        Time.timeScale = 0f; // Pause du jeu
    }

    public void OpenEndMenu()
    {
        if (EnemyManager.Instance != null) EnemyManager.Instance.Stop();
        m_PanelEndMenu.SetActive(true);
        currentPanel = m_PanelEndMenu;

        if (m_ScoreText != null)
        {
            if (ScoreManager.Instance != null)
            {
                float score = ScoreManager.Instance.Precision;
                m_ScoreText.text = "SCORE DE " + score.ToString("F1") + "%"; 
            }
            else
            {
                m_ScoreText.text = "SCORE : --";
            }
        }

        Time.timeScale = 0f;
    }

    public void OpenCommandMenu()
    {
        CloseAllPanels();
        m_PanelCommandMenu.SetActive(true);
        currentPanel = m_PanelCommandMenu;
    }

    void CloseAllPanels()
    {
        m_PanelMainMenu.SetActive(false);
        m_PanelPauseMenu.SetActive(false);
        m_PanelGameoverMenu.SetActive(false);
        m_PanelEndMenu.SetActive(false);
        m_PanelCommandMenu.SetActive(false);
    }

    public void LoadLevel(string LevelName)
    {
        Debug.Log("LoadLevel: " + LevelName);
        selectedLevel = LevelName;
        SceneManager.LoadScene(LevelName, LoadSceneMode.Additive);
        m_PanelMainMenu.SetActive(false);
    }

    public void RestartLevel()
    {
        if (EnemyManager.Instance != null) EnemyManager.Instance.DestroyAllEnemies();
        CloseAllPanels();
        SceneManager.UnloadSceneAsync(selectedLevel);
        SceneManager.LoadScene(selectedLevel, LoadSceneMode.Additive);
        Time.timeScale = 1f;
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
        if (EnemyManager.Instance != null) EnemyManager.Instance.DestroyAllEnemies();
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(selectedLevel);
        OpenMainMenu();
    }

    public void SetEasyMode(bool isEnabled)
    {
        IsEasyMode = isEnabled;
        Debug.Log("Mode Facile : " + IsEasyMode);
    }
}
