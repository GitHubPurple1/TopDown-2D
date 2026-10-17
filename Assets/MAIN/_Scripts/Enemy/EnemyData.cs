using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private float contactDamage;
    public float ContactDamage => contactDamage;
    [SerializeField] private int scoreValue;
    public int ScoreValue => scoreValue;
    [SerializeField] private Color color;
    public Color Color => color;
}