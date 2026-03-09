using System.Reflection;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private Player player;

    private ProjectileData data;
    private ProjectilePool myPool;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private ParticleSystem trail;

    // TDD
    public void SetPlayer(Player playerReference)
    {
        player = playerReference;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponentInChildren<ParticleSystem>();
    }

    public void Initialize(ProjectileData incomingData)
    {
        data = incomingData;

        spriteRenderer.sprite = data.projectileSprite;
        
        trail.Stop();
        trail.Clear();

        if (data.hasTail)
        {

            var tsam = trail.textureSheetAnimation;
            tsam.SetSprite(0, data.projectileSprite);
            tsam.enabled = true;

            var main = trail.main;
            main.startColor = data.tailColor;
            main.startSizeMultiplier = data.projectileScale * data.trailSizeMultiplier;

            trail.Play();
        }

        // Scale the snowball down
        transform.localScale = Vector3.one * data.projectileScale;
        
        // Zero all previous velocities
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0.0f;

        rb.AddForce(new(data.TotalSpeed, 0.0f));
        rb.AddTorque(-data.TotalRotation);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.TakeDamage(player.TotalDamage());
            Death();
        }
    }

    public void SetPool(ProjectilePool pool)
    {
        myPool = pool;
    }

    public void Death()
    {
        if (data.explosionEffect != null)
        {
            Instantiate(data.explosionEffect, transform.position, Quaternion.identity);
        }
        if (data.explosionSound != null)
        {
            AudioManager.instance.PlaySound(data.explosionSound, transform.position);
        }

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

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
