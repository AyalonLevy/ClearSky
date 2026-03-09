using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float parallaxEffect; // The speed at which the background should move relative to the camera/player
    public float movementX = 1.0f;  // The speed at which the player is "moving"
    public float playerSpeed = 15.0f;
    public bool canChangeHeightPos = false;

    private float startPosX;
    private float startPosY;
    private float length;
    private float posY;

    private void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        posY = GetYPos();
    }

    private void FixedUpdate()
    {
        float currentPos = transform.position.x;

        // Calculate "new" x and y position
        if (currentPos > startPosX + length)
        {
            currentPos = startPosX - length;

            posY = GetYPos();
        }
        else if (currentPos < startPosX - length)
        {
            currentPos = startPosX + length;

            posY = GetYPos();
        }

        float movement = currentPos - playerSpeed * parallaxEffect * Time.deltaTime;
        transform.position = new(movement, posY, transform.position.z);
    }

    private float GetYPos()
    {
        if (canChangeHeightPos)
        {
            return Random.Range(startPosY, 0.0f);
        }
        else
        {
            return transform.position.y;
        }
    }
}
