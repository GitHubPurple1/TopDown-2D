using UnityEngine;
[RequireComponent (typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float attackInterval;
    [SerializeField] private LayerMask targetLayers;
    private float nextDamageTime;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;
        if ((targetLayers.value & (1 << collision.gameObject.layer)) == 0) return;
        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            nextDamageTime = Time.time + attackInterval;
        }
        

    }

    public void Initialize(float damageValue)
    {
        damage = damageValue;
        nextDamageTime = Time.time + attackInterval; // Pooling Enemies & grace period before attack
    }
}