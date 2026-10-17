using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyData fastData;
    [SerializeField] private EnemyData tankData;

    [Header("Difficulty")]
    [SerializeField] private float startInterval = 2f;
    [SerializeField] private float minInterval = 0.4f;
    [SerializeField] private float startTankChance = 0.1f;
    [SerializeField] private float maxTankChance = 0.4f;
    [SerializeField] private float rampDuration = 120f;   // seconds to reach max difficulty

    [SerializeField] private float spawnMargin = 2f;

    private Camera cam;
    private float nextSpawnTime;
    private float startTime;

    public event System.Action<EnemyData> OnEnemyKilled;

    private void Awake() 
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Camera Reference Not Found");
            this.enabled = false;
        }
    }

    private void OnEnable() 
    { 
       startTime = Time.time;

       nextSpawnTime = Time.time + startInterval;
    }

    private void Update()
    {
        if (player == null) return;
        if (Time.time < nextSpawnTime) return;

        float t = Mathf.Clamp01((Time.time - startTime) / rampDuration);
        float interval =  Mathf.Lerp(startInterval, minInterval, t) ;

        Spawn(t);
        nextSpawnTime = Time.time + interval;
    }

    private void Spawn(float difficulty)
    {
        EnemyData enemyData;

        float chance = Mathf.Lerp(startTankChance, maxTankChance, difficulty);
        if (Random.value < chance)
        {
            enemyData = tankData;
        }
        else
        {
            enemyData = fastData;
        }

        Vector2 spawnPos = GetSpawnPosition();
        EnemyController enemy = Instantiate(enemyPrefab,spawnPos,Quaternion.identity);

        enemy.Initialize(enemyData,player);

        enemy.OnEnemyDied += HandleEnemyDied;
    }

    private Vector2 GetSpawnPosition()
    {
        // Screen Math
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        
        float distanceToCorner = Mathf.Sqrt(Mathf.Pow(halfHeight, 2) + Mathf.Pow(halfWidth, 2));
        float radius = distanceToCorner + spawnMargin;

        Vector2 randomDirection = Random.insideUnitCircle.normalized * radius;
        Vector2 randomPos = randomDirection + (Vector2)player.position;

        return randomPos;
    }

    private void HandleEnemyDied(EnemyData data)
    {
        OnEnemyKilled?.Invoke(data);
    }
}