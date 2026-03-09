using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Will destroy any gameObject that leaves it's domain
    public float sizeThreshold = 1.2f;
    public ObstaclePool obstaclePool;
    public ProjectilePool projectilePool;
    public PowerUpPool powerUpPool;

    private void Start()
    {
        // Calculate death ray position
        Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();

        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        boxCol.size = new Vector2(spawner.screenWidth * sizeThreshold, spawner.screenHeight * sizeThreshold);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Obstacle obstacle))
        {
            obstaclePool.ReturnObject(obstacle);
        }
        else if (collision.TryGetComponent(out Projectile projectile))
        {
            projectilePool.ReturnObject(projectile);
        }
        else if (collision.TryGetComponent(out PowerUp powerUp))
        {
            powerUpPool.ReturnObject(powerUp);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
