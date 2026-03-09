using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/Projectile")]
public class ProjectileData : ScriptableObject
{
    [Header("Configuration")]
    public Sprite projectileSprite;
    public ParticleSystem explosionEffect;
    public AudioEvent explosionSound;
    public float projectileScale = 0.5f;
    public float projectileRotation = 50.0f;
    public float projectileSpeed = 40.0f;
    public float speedMultiplier = 10.0f;
    public float damage = 10.0f;

    [Header("Tail Configuration")]
    public bool hasTail = false;
    public Color tailColor = Color.white;
    public float trailSizeMultiplier = 1.0f;

    public float TotalSpeed { get => projectileSpeed * speedMultiplier; }
    public float TotalRotation { get => projectileRotation * speedMultiplier; }
}
