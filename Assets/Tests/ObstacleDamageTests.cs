using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ObstacleDamageTests
{
    [UnityTest]
    public IEnumerator ObstacleDamageTestsWithEnumeratorPasses()
    {
        // Create a blank GameObject and add the necessary components
        GameObject testObject = new("TestObstacle");
        
        testObject.AddComponent<SpriteRenderer>();
        testObject.AddComponent<Rigidbody2D>();

        // Add the Obstacle component
        Obstacle obstacle = testObject.AddComponent<Obstacle>();

        // Creae dummy ScriptableObject data
        ObstacleData dummyData = ScriptableObject.CreateInstance<ObstacleData>();
        dummyData.health = 50.0f;
        
        // Make sure the size doesn't change => won't change the health amount
        dummyData.minSize = 1.0f;
        dummyData.maxSize = 1.0f;
        dummyData.gravityScale = 1.0f;

        // Inject the data
        obstacle.Initialize(dummyData);

        // Verify initialization
        Assert.AreEqual(50.0f, obstacle.CurrentHealth, "Obstacle did not initialize health correctly");

        obstacle.TakeDamage(20.0f);

        yield return null;

        // Check if health changed according to damage
        Assert.AreEqual(30.0f, obstacle.CurrentHealth, "Obstacle health did not reduce by the correct amount");

        // Cleanup
        Object.Destroy(testObject);
    }
}
