using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player Configuration")]
    public int health = 10;
    public int maxHealth = 10;
    public float invaluablityTimeout = 1.0f;
    public float speed = 1.0f;
    public float fireRate = 1.0f;
    public float playerBonusDamage = 0.0f;
    public int ammo = 10;
    public int maxAmmo = 20;
    public float reloadTime = 3.0f;
    public float maxLoadingRate = 0.5f;
    public AudioEvent hitSound;
    public Image reloadImage;
    public float reloadRotationSpeed;

    [Header("Projectile Configuration")]
    public ProjectileData[] projectileData;
    public ProjectilePool snowballPool;
    public int initialProjectileIndex = 0;

    [Header("Game Parameters")]
    public Vector2 moveBoundariesX = new(0.1f, 0.4f);
    public CameraShake cameraShake;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int currentAmmo;

    private bool isReloading = false;
    private bool isInvaluable = false;
    private float invaluableStartTime = 0.0f;
    private float lastThrow;
    private float currentAmmoDamage;
    private Transform snowballSpawner;
    private Animator animator;
    private ProjectileData currentProjectile;

    private readonly float shakeDuration = 0.15f;
    private readonly float shakeMagnitude = 0.15f;


    private void Awake()
    {
        animator = GetComponent<Animator>();

        snowballSpawner = transform.Find("SnowballSpawner");
        lastThrow = Time.time;

        reloadImage.enabled = false;
    }

    void Start()
    {
        currentHealth = health;
        currentAmmo = ammo;
        UpdateCurrentAmmo(initialProjectileIndex);

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateHealth();
            GameManager.instance.UpdateAmmo();
        }
    }

    void Update()
    {
        if (isInvaluable)
        {
            if (Time.time - invaluableStartTime >= invaluablityTimeout)
            {
                TurnInvaluableOff();
            }
        }

        if (currentAmmo <= 0 && !isReloading)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.Death();
            GetHit(obstacle.GetData().damage);

            // Maybe don't reward for headbutting the obstacles
            GameManager.instance.AddScore();
        }

        if (collision.TryGetComponent(out PowerUp powerUp))
        {
            ApplyPoweredUp(powerUp);
        }
    }

    public void UpdateCurrentAmmo(int index)
    {
        // Verify that the "Upgraded" projectile is not out of range of the current pool of defined projectiles
        index = Mathf.Min(index, projectileData.Length - 1);

        currentProjectile = projectileData[index];
        currentAmmoDamage = currentProjectile.damage;

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateDamage();
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = ammo;
        GameManager.instance.UpdateAmmo();
        isReloading = false;
    }

    private void GetHit(int damage)
    {
        if (isInvaluable)
        {
            return;
        }

        currentHealth -= damage;
        GameManager.instance.UpdateHealth();

        AudioManager.instance.PlaySound(hitSound, transform.position);
        StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));

        if (currentHealth <= 0)
        {
            Death();
        }

        TurnInvaluableOn();
    }

    private void Death()
    {
        Destroy(gameObject);

        GameManager.instance.GameOver();
    }

    private void TurnInvaluableOn()
    {
        isInvaluable = true;
        invaluableStartTime = Time.time;

        // Add blinking effect
        animator.SetLayerWeight(1, 1.0f);
    }

    private void TurnInvaluableOff()
    {
        isInvaluable = false;

        // Remove blinking effect
        animator.SetLayerWeight(1, 0.0f);
    }

    public void ThrowSnowball()
    {
        if (isReloading)
        {
            StartCoroutine(ReloadAnimation());
            return;
        }


        if (Time.time - lastThrow < 1.0f / fireRate)
        {
            return;
        }

        animator.SetTrigger("Throw");
        lastThrow = Time.time;
    }

    private void SpawnSnowball()
    {
        Projectile sb = snowballPool.GetObject();
        sb.transform.SetPositionAndRotation(snowballSpawner.position, Quaternion.identity);

        ProjectileData selectedAmmo = currentProjectile;
        sb.SetPlayer(this);
        sb.Initialize(selectedAmmo);
        sb.SetPool(snowballPool);

        currentAmmo--;
        GameManager.instance.UpdateAmmo();
        animator.ResetTrigger("Throw");
    }

    private IEnumerator ReloadAnimation()
    {
        reloadImage.enabled = true;

        while (isReloading)
        {
            Vector3 rotation = new(0.0f, 0.0f, reloadRotationSpeed * Time.deltaTime);
            reloadImage.transform.Rotate(rotation);

            yield return null;
        }

        reloadImage.enabled = false;
    }

    public float TotalDamage()
    {
        return currentAmmoDamage + playerBonusDamage;
    }

    private void ApplyPoweredUp(PowerUp powerUp)
    {
        PowerUpData data = powerUp.GetData();

        switch (data.powerUpType)
        {
            case PowerUpData.PowerUpsTypes.Power:
                playerBonusDamage += data.statIncrease;
                GameManager.instance.UpdateDamage();
                break;
            case PowerUpData.PowerUpsTypes.ReloadTime:
                reloadTime -= (1.0f / data.statIncrease);
                reloadTime = Mathf.Max(reloadTime, maxLoadingRate);
                GameManager.instance.UpdateReloadTime();
                break;
            case PowerUpData.PowerUpsTypes.Ammo:
                currentAmmo += data.statIncrease;
                if (currentAmmo > ammo)
                {
                    ammo = Mathf.Min(currentAmmo, maxAmmo);
                }
                GameManager.instance.UpdateAmmo();
                break;
            case PowerUpData.PowerUpsTypes.Health:
                currentHealth += data.statIncrease;
                if (currentHealth > health)
                {
                    health = Mathf.Min(currentHealth, maxHealth);
                }
                GameManager.instance.UpdateHealth();
                break;
        } 

        powerUp.Death();
    }
}
