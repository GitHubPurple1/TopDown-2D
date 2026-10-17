using System;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(HealthComponent))]
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float damage;

    private InputReader reader;
    private float nextFireTime;

    public event Action OnShot;

    private void Awake()
    {
        reader = GetComponent<InputReader>();
    }

    private void Update()
    {
        if (!reader.GetShootingState()) return;   // not holding the button
        if (Time.time < nextFireTime) return;

        Shoot();
        nextFireTime = Time.time + fireCooldown;
    }

    private void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position,firePoint.rotation);
        projectile.Initialize(firePoint.right,projectileSpeed,damage);
        OnShot?.Invoke();
    }
}