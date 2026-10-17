using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button restartButton;

    private void Awake() 
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        gameManager.OnGameOver += Show;
        restartButton.onClick.AddListener(gameManager.RestartGame);
    }

    private void OnDisable() 
    {
        gameManager.OnGameOver -= Show;
        restartButton.onClick.RemoveListener(gameManager.RestartGame);
    }

    private void Show() 
    { 
        finalScoreText.text = "Score : " + gameManager.Score.ToString();
        panel.SetActive(true);
    }
}