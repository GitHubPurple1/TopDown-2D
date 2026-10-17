using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float lifetime;
    private float damageValue;
    private float timeToDespawn;
    private bool hasHit;
    #region Unity
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hasHit = true;
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
        hasHit = false;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}