using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(ContactDamage))]
public class EnemyController : MonoBehaviour
{
    private EnemyData currentData;
    private HealthComponent health;
    private Rigidbody2D rb;
    private SpriteRenderer visualSprite;
    private Transform playerTransform;
    private ContactDamage contactDamage;
    private const float MinAimDistanceSqr = 0.1f;
    public event Action<EnemyData> OnEnemyDied;
    #region Unity
    private void Awake()
    {
        health = GetComponent<HealthComponent>();
        rb = GetComponent<Rigidbody2D>();
        visualSprite = GetComponentInChildren<SpriteRenderer>();
        contactDamage = GetComponent<ContactDamage>();
    }
    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }
    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }
    private void FixedUpdate()
    {
        if (playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        // Movement
        Vector2 targetPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        Vector2 direction = (targetPosition - rb.position);
        if (direction.sqrMagnitude < MinAimDistanceSqr)
        {
            // Zero distance edge case
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = direction.normalized * currentData.MoveSpeed;

        // Rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // +X is forward
        rb.MoveRotation(angle);
    }
    #endregion
    public void Initialize(EnemyData data, Transform target)
    {
        rb.linearVelocity = Vector2.zero;
        playerTransform = target;
        currentData = data;
        health.Initialize(currentData.MaxHealth);
        visualSprite.color = data.Color;
        contactDamage.Initialize(currentData.ContactDamage);
    }

    private void HandleDeath()
    {
        OnEnemyDied?.Invoke(currentData);
    }  
}