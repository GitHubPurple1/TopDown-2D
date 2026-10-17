using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, GameOver }

public class GameManager : MonoBehaviour
{
    [SerializeField] private HealthComponent playerHealth;
    [SerializeField] private InputReader playerInput;
    [SerializeField] private EnemySpawner spawner;

    public int Score { get; private set; }
    public GameState State { get; private set; }
    public float SurvivalTime => Time.time - startTime;

    public event Action<int> OnScoreChanged;
    public event Action OnGameOver;

    private float startTime;
    #region Unity
    private void Awake()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        startTime = Time.time;
    }

    private void OnEnable()
    {
        playerHealth.OnDeath += HandlePlayerDeath;
        spawner.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        playerHealth.OnDeath -= HandlePlayerDeath;
        spawner.OnEnemyKilled -= HandleEnemyKilled;
    }
    #endregion
    private void HandleEnemyKilled(EnemyData data)
    {
        if(State!= GameState.Playing) return;
        Score += data.ScoreValue;
        OnScoreChanged?.Invoke(Score);
    }

    private void HandlePlayerDeath()
    {
        State = GameState.GameOver;
        playerInput.enabled = false;
        OnGameOver?.Invoke();
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}