using System;
using UnityEngine;
using UnityEngine.Pool;
[RequireComponent(typeof(InputReader))]
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float damage;

    private ObjectPool<Projectile> pool;
    private InputReader reader;
    private float nextFireTime;

    public event Action OnShot;

    #region Unity
    private void Awake()
    {
        reader = GetComponent<InputReader>();

        pool = new ObjectPool<Projectile>(
        createFunc: () =>
        {
            Projectile p = Instantiate(projectilePrefab);
            p.OnDespawn += pool.Release;
            return p;
        },      
        actionOnGet: p => p.gameObject.SetActive(true),       
        actionOnRelease: p => p.gameObject.SetActive(false),  
        actionOnDestroy: p => Destroy(p.gameObject),
        collectionCheck: true,
        defaultCapacity: 20,
        maxSize: 100);
    }

    private void Update()
    {
        if (!reader.GetShootingState()) return;   // not holding the button
        if (Time.time < nextFireTime) return;

        Shoot();
        nextFireTime = Time.time + fireCooldown;
    }

    private void OnDestroy()
    {
        pool.Dispose();
    }
    #endregion
    private void Shoot()
    {
        Projectile projectile = pool.Get();
        projectile.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        projectile.Initialize(firePoint.right,projectileSpeed,damage);
        OnShot?.Invoke();
    }
}