using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private HealthComponent playerHealth;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    private int lastShownSecond = -1;

    private void OnEnable()
    {
        playerHealth.OnDamageTaken += UpdateHealth;
        gameManager.OnScoreChanged += UpdateScore;
    }

    private void OnDisable() 
    {
        playerHealth.OnDamageTaken -= UpdateHealth;
        gameManager.OnScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        UpdateHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        UpdateScore(gameManager.Score);
    }

    private void Update()
    {
        int seconds = Mathf.FloorToInt(gameManager.SurvivalTime);
        if (seconds == lastShownSecond) return;
        lastShownSecond = seconds;
        timeText.text = $"{lastShownSecond/60:00}:{lastShownSecond%60:00}";
    }

    private void UpdateHealth(float current, float max)
    {
        hpSlider.value = current / max;
        hpText.text = "HEALTH " +current + "/" + max;
    }
    private void UpdateScore(int score) 
    {
       scoreText.text = score.ToString();
    }
}