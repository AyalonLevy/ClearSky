using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Obstacle Configuration")]
    public ObstaclePool obstaclePool;
    public ObstacleData[] obstacleData;

    [Header("Power Up Configuration")]
    public PowerUpPool powerUpPool;
    public PowerUpData[] powerUpData;

    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;

    public int powerUpSpawnCounter = 20;

    [Header("General Configuration")]
    public float minSpawnInterval = 1.0f;
    public float maxSpawnInterval = 3.0f;
    public float difficultyScale = 0.0f;

    private GameManager GM => GameManager.instance;
    private float nextSpawnTime;
    private int obstacleSpawnedCounter;
    private float instantiationPosX = 0.0f;
    private float instantiationPosY = 10.0f;

    private readonly float spawnScreenOffset = 1.2f;

    [Header("Difficulty Progression")]
    [SerializeField] private float speedMultiplierPerLevel = 0.9f;  // 10% faster
    [SerializeField] private float absoluteMinSpawnInterval = 0.4f;  //  to prevent thew spawn rate to be impossible for the player to play

    private float initialMinInterval;
    private float initialMaxInterval;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        GetScreenInWorldCoordinates();
    }

    private void Start()
    {
        initialMinInterval = minSpawnInterval;
        initialMaxInterval = maxSpawnInterval;

        nextSpawnTime = Time.time + GetRandomValue(minSpawnInterval, maxSpawnInterval);
        obstacleSpawnedCounter = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GM.isPaused)
        {
            nextSpawnTime += Time.deltaTime;
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle(GetSpawnPosition());
        }

        if (obstacleSpawnedCounter >= powerUpSpawnCounter)
        {
            SpawnPowerUp(GetSpawnPosition());
            obstacleSpawnedCounter = 0;
        }
    }

    public void IncreaseLevelDifficulty(int level)
    {
        float currentMultiplier = Mathf.Pow(speedMultiplierPerLevel, level - 1);

        minSpawnInterval = Mathf.Max(initialMinInterval * currentMultiplier, absoluteMinSpawnInterval);
        maxSpawnInterval = Mathf.Max(initialMaxInterval * currentMultiplier, absoluteMinSpawnInterval + 0.5f);
    }

    private void GetScreenInWorldCoordinates()
    {
        Vector2 screenBoundaries;

        Vector3 originInCameraSpace = Utils.ScreenToWorld(Camera.main, Vector3.zero);

        screenBoundaries.x = originInCameraSpace.x * -2.0f;
        screenBoundaries.y = originInCameraSpace.y * -2.0f;

        screenWidth = screenBoundaries.x;
        screenHeight = screenBoundaries.y;
    }

    private float GetRandomValue(float minValue, float maxValue)
    {
        return Random.Range(minValue, maxValue);
    }

    private Vector3 GetSpawnPosition()
    {
        instantiationPosX = screenWidth / 2.0f * spawnScreenOffset;
        instantiationPosY = GetRandomValue(-screenHeight / 2.0f, screenHeight / 2.0f);

        return new(instantiationPosX, instantiationPosY, 0.0f);
    }

    private void SpawnObstacle(Vector3 spawnPosition)
    {
        // Instantiate a blank obstacle
        Obstacle obstacle = obstaclePool.GetObject();
        obstacle.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        // Set obstacle and initialize it
        //ObstacleData selectedObstacle = obstacleData[Random.Range(0, obstacleData.Length)];
        ObstacleData selectedObstacle = GetWeightedItem(obstacleData, x => x.spawnWeight);
        obstacle.Initialize(selectedObstacle);
        obstacle.SetPool(obstaclePool);

        // Add health based on difficulty
        obstacle.AddHealth(difficultyScale);

        // Update spawn parameters
        nextSpawnTime = Time.time + GetRandomValue(minSpawnInterval, maxSpawnInterval);
        obstacleSpawnedCounter++;
    }

    private void SpawnPowerUp(Vector3 spawnPosition)
    {
        // Instantiate a blank powerup
        PowerUp powerUp = powerUpPool.GetObject();
        powerUp.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        // Set powerup and initialize it
        PowerUpData selectedPowerUp = GetWeightedItem(powerUpData, x => x.spawnWeight);
        powerUp.Initialize(selectedPowerUp);
        powerUp.SetPool(powerUpPool);

        // Update spawn parameters
        nextSpawnTime = Time.time + GetRandomValue(minSpawnInterval, maxSpawnInterval);
        
        // Increase the difficulty everytime a power up is spawned
        difficultyScale += 1;
    }

    private T GetWeightedItem<T>(T[] items, System.Func<T, float> weightSelector)
    {
        float totalWeight = 0.0f;

        // Calculate the sum of all weights
        foreach (var item in items)
        {
            totalWeight += weightSelector(item);
        }

        // Pick a random number between 0 and the total weight
        float randomValue = Random.Range(0, totalWeight);

        // Iterate through again to find which "bracket" the random value fell into
        float cumulativeWeight = 0.0f;
        foreach (var item in items)
        {
            cumulativeWeight += weightSelector(item);
            if (randomValue <= cumulativeWeight)
            {
                return item;
            }
        }

        // Fall back
        return items[0];
    }
}
