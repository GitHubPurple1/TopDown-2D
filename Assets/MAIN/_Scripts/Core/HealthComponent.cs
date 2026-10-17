using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour,IDamageable
{
    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public event Action OnDeath;
    public event Action<float,float> OnDamageTaken; // Passes current Health & Max Health

    #region Unity

    private void Awake()
    {
        Initialize(maxHealth);
    }

    #endregion

    public void Initialize(float health)
    { 
        maxHealth= health;
        ResetHealth();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead|| damage < 0) return;

        // Damage
        CurrentHealth = Mathf.Max(CurrentHealth -damage,0);
        OnDamageTaken?.Invoke(CurrentHealth,maxHealth);

        // Death Check
        if (CurrentHealth <= 0)
        {
            IsDead = true;
            OnDeath?.Invoke();
        }

    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }
}