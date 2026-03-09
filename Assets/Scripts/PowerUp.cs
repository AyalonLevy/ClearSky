using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private PowerUpData data;
    private PowerUpPool myPool;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(PowerUpData incomingData)
    {
        data = incomingData;

        spriteRenderer.sprite = data.powerUpSprite;
        rb.gravityScale /= data.gravityScale;
    }

    public void SetPool(PowerUpPool pool)
    {
        myPool = pool;
    }

    public PowerUpData GetData()
    {
        return data;
    }

    public void Death()
    {
        if (myPool != null)
        {
            myPool.ReturnObject(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if (data.visualEffect != null)
        {
            Instantiate(data.visualEffect, transform.position, Quaternion.identity);
        }
        
        if (data.collectedSound != null)
        {
            data.collectedSound.Play();
        }
    }
}
