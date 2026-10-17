using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float lifetime;
    private float damageValue;
    private float timeToDespawn;
    private bool isFinished;
    public event Action<Projectile> OnDespawn;

    #region Unity
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFinished) return;

        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damageValue);
        }
        Despawn();
    }

    private void Update()
    {
        if (Time.time >= timeToDespawn) Despawn();
    }
    #endregion

    public void Initialize(Vector2 direction, float speed, float damage)
    {
        damageValue = damage;
        rb.linearVelocity = direction.normalized * speed;
        timeToDespawn = Time.time + lifetime;
        isFinished = false;
    }

    private void Despawn()
    {
        if(isFinished) return;
        isFinished = true;
        rb.linearVelocity = Vector2.zero;
        OnDespawn?.Invoke(this);
    }
}