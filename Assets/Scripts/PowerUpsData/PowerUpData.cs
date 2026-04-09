using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpData", menuName = "Scriptable Objects/PowerUp")]
public class PowerUpData : ScriptableObject
{
    public enum PowerUpsTypes
    {
        Power,
        ReloadTime,
        Ammo,
        Health
    }

    public PowerUpsTypes powerUpType;

    public Sprite powerUpSprite;
    public ParticleSystem visualEffect;
    public AudioSource collectedSound;

    public int statIncrease = 1;
    public float gravityScale = 5.0f;

    public float spawnWeight = 1.0f;
}
