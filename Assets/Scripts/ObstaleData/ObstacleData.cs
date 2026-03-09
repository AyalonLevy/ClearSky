using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Scriptable Objects/Obstacle")]
public class ObstacleData : ScriptableObject
{
    [Header("Configuration")]
    public Sprite obstacleSprite;
    public ParticleSystem explosionEffect;
    public AudioEvent explosionSound;
    public int damage;
    public float health;
    public float minSize = 0.3f;
    public float maxSize = 1.5f;

    [Tooltip("Gravity effect multiplier")]
    public float gravityScale;

    [Header("Tail Configuration")]
    public bool hasTail = false;
    public Gradient tailGradient;
    public float trailSizeMultiplier = 1.0f;
}
