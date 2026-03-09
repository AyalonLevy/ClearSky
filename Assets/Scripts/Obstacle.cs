using UnityEngine;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    private ObstacleData data;
    private float currentHealth;
    private ObstaclePool myPool;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private ParticleSystem trail;

    // TDD
    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponentInChildren<ParticleSystem>();
    }

    public void Initialize(ObstacleData incomingData)
    {
        data = incomingData;

        spriteRenderer.sprite = data.obstacleSprite;

        // Randomize size and "mass" -> moving speed
        float s = Random.Range(data.minSize, data.maxSize);

        transform.localScale = Vector3.one * s;
        rb.gravityScale = 1.0f / (s * data.gravityScale);

        currentHealth = Mathf.Max(data.health, data.health * s);

        trail.Stop();
        trail.Clear();

        if (data.hasTail)
        {
            var main = trail.main;
            main.startSizeMultiplier = s * data.trailSizeMultiplier;

            var shape = trail.shape;
            shape.radius = s * data.trailSizeMultiplier;

            var colorModule = trail.colorOverLifetime;
            colorModule.enabled = true;
            colorModule.color = new ParticleSystem.MinMaxGradient(data.tailGradient);

            trail.Play();
        }

        // Randomize rotation
        float r = Random.Range(0, 360);
        transform.Rotate(new(0.0f, 0.0f, r));
        rb.AddTorque(r / 10);
    }

    public ObstacleData GetData()
    {
        return data;
    }

    public void SetPool(ObstaclePool pool)
    {
        myPool = pool;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GameManager.instance.AddScore();
            Death();
        }
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;
    }

    public void Death()
    {
        if (data.explosionEffect != null)
        {
            Instantiate(data.explosionEffect, transform.position, Quaternion.identity);
        }

        AudioManager.instance.PlaySound(data.explosionSound, transform.position);

        if (myPool != null)
        {
            myPool.ReturnObject(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
