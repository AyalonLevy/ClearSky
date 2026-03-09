using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileTests
{
    private Player player;
    private GameObject playerObj;
    private ProjectileData projectileData;

    [SetUp]
    public void Setup()
    {
        // Create a dummy GameManager (for the Player)
        GameObject gmObj = new("TestGameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();

        // Create the player
        playerObj = new("TestPlayer");
        playerObj.AddComponent<Animator>();
        player = playerObj.AddComponent<Player>();
        player.playerBonusDamage = 10.0f;
        player.currentHealth = 10;
        player.currentAmmo = 10;
        player.reloadTime = 3.0f;
        player.initialProjectileIndex = 0;
        player.maxAmmo = 10;
        player.maxHealth = 10;

        // Create the porjectile dummy data
        projectileData = ScriptableObject.CreateInstance<ProjectileData>();
        projectileData.damage = 5.0f;
        projectileData.projectileSpeed = 1.0f;
        projectileData.projectileRotation = 1.0f;
        projectileData.speedMultiplier = 10.0f;

        player.projectileData = new ProjectileData[] { projectileData };

        player.UpdateCurrentAmmo(0);
    }

    [TearDown]
    public void TearDown()
    {
        GameManager.instance = null;
        UIManager.instance = null;

        // Clean up after every test
        Object.Destroy(playerObj);
        Object.Destroy(GameObject.Find("TestGameManager"));
        Object.Destroy(GameObject.Find("TestUIManager"));

        foreach (var obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.name.Contains("Test"))
            {
                Object.Destroy(obj);
            }
        }
    }

    [UnityTest]
    public IEnumerator ProjectileHitObstacleAppliesCombinedDamage()
    {
        // Setup the obstacle
        GameObject obstacleObj = new("TestObstacle")
        {
            tag = "Obstacle"
        };
        obstacleObj.AddComponent<SpriteRenderer>();
        obstacleObj.AddComponent<Rigidbody2D>();
        BoxCollider2D obstacleCollider  = obstacleObj.AddComponent<BoxCollider2D>();
        Obstacle obstacle = obstacleObj.AddComponent<Obstacle>();

        // Setup obstacle dummy data
        ObstacleData obstacleData = ScriptableObject.CreateInstance<ObstacleData>();
        obstacleData.health = 50.0f;
        obstacleData.minSize = 1.0f;
        obstacleData.maxSize = 1.0f;
        obstacleData.gravityScale = 1.0f;
        obstacle.Initialize(obstacleData);

        // Setup the projectile
        GameObject projectileObj = new("TestProjectile")
        {
            tag = "Projectile"
        };
        projectileObj.AddComponent<SpriteRenderer>();
        projectileObj.AddComponent<Rigidbody2D>();
        projectileObj.AddComponent<CircleCollider2D>().isTrigger = true;
        Projectile projectile = projectileObj.AddComponent<Projectile>();

        projectile.Initialize(projectileData);
        projectile.SetPlayer(player);

        // Force collision
        projectile.OnTriggerEnter2D(obstacleCollider);

        yield return new WaitForFixedUpdate();

        // ASSERT
        float expectedHealth = 35.0f;
        Assert.AreEqual(expectedHealth, obstacle.CurrentHealth, "The obstacle did not take the correct amount of damage");
    }

    [UnityTest]
    public IEnumerator ProjectileInitializeAppliesCorrectVelocity()
    {
        GameObject projectileObj = new("PhysicsProjectile");
        projectileObj.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = projectileObj.AddComponent<Rigidbody2D>();
        Projectile projectile = projectileObj.AddComponent<Projectile>();

        ProjectileData projectileData = ScriptableObject.CreateInstance<ProjectileData>();

        projectile.Initialize(projectileData);

        yield return new WaitForFixedUpdate();

        // ASSERT
        Assert.AreNotEqual(Vector2.zero, rb.linearVelocity, "Projectile should have velocity after initialization");

        // Clean up
        Object.Destroy(projectileObj);
    }
}
