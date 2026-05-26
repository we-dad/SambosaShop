using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Start and gameOver panel, Owns the start button and the restart button
/// </summary>
public class PanelUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject StartPanel;
    public TextMeshProUGUI scoreText;
    
    public UnityEngine.UI.Button restartButton;
    public UnityEngine.UI.Button startButton;
    
    void Start()
    {
        GameStateMachine.Instance.OnStateChanged += HandleStateChanged;
        gameOverPanel.SetActive(false);
        StartPanel.SetActive(true);
        
        restartButton.onClick.AddListener(RestartGame);
        startButton.onClick.AddListener(StartTheGame);
    }
    
    void OnDestroy()
    {
        if (GameStateMachine.Instance != null)
            GameStateMachine.Instance.OnStateChanged -= HandleStateChanged;
    }
    
    private void HandleStateChanged(SambosaGameState newState)
    {
        if (newState == SambosaGameState.GameOver)
        {
            int served = OrderManager.Instance.CustomersServed;
            int passed = OrderManager.Instance.CustomersPassed;
            scoreText.text = "You served " + passed + " out of " + served;
            gameOverPanel.SetActive(true);
        }
        
        if (newState == SambosaGameState.Making)
        {
            StartPanel.SetActive(false);
        }
    }

    public void StartTheGame()
    {
        OrderManager.Instance.StartGame();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}