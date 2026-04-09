using System.Collections;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Camera mainCamera;

    // Player related settings
    private float playerSpeed;
    private Vector2 moveBoundariesX;

    private Vector2 screenBoundaries;
    private Coroutine moveCoroutine;

    private float touchStartTime;
    private readonly float touchOffsetX = 2.0f;
    private const float tapTimeThreshold = 0.15f;
    private bool isProtentialTap;

    private void Awake()
    {
        mainCamera = Camera.main;

        playerSpeed = player.GetComponent<Player>().speed;
        moveBoundariesX = player.GetComponent<Player>().moveBoundariesX;
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        GameObject spawner = GameObject.Find("Spawner");
        screenBoundaries.x = spawner.GetComponent<Spawner>().screenWidth;
        screenBoundaries.y = spawner.GetComponent<Spawner>().screenHeight;
    }

    private void Update()
    {
        if (GameManager.instance.isPaused)
        {
            return;
        }
        foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time;
                isProtentialTap = true;
            }

            if (touch.inProgress)
            {
                if (Time.time - touchStartTime > tapTimeThreshold)
                {
                    isProtentialTap = false;
                    FollowTouch(touch);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (isProtentialTap && !GameManager.instance.useButtonControl)
                {
                    PerformAction();
                }
            }
        }
    }

    private void FollowTouch(EnhancedTouch.Touch touch)
    {
        moveCoroutine ??= StartCoroutine(MoveToTarget(touch));
    }

    private IEnumerator MoveToTarget(EnhancedTouch.Touch touch)
    {
        Vector3 target;

        //while (touchAction.Player.Move.IsPressed())
        while (touch.inProgress && !touch.isTap && player != null)
        {
            target = Utils.ScreenToWorld(mainCamera, touch.screenPosition);

            target = CheckIfReachedBoundary(target);

            player.transform.position = Vector3.MoveTowards(player.transform.position, target, playerSpeed * Time.deltaTime);

            yield return null;
        }

        moveCoroutine = null;
    }

    private Vector3 CheckIfReachedBoundary(Vector3 target)
    {
        // From geometry, after some simplification
        float minBoundaryX = screenBoundaries.x * (moveBoundariesX.x - 0.5f);
        float maxBoundaryX = screenBoundaries.x * (moveBoundariesX.y - 0.5f);

        target.x = Mathf.Max(target.x + touchOffsetX, minBoundaryX);
        target.x = Mathf.Min(target.x + touchOffsetX, maxBoundaryX);

        target.y = Mathf.Max(target.y, (-screenBoundaries.y + player.transform.localScale.y) / 2.0f);
        target.y = Mathf.Min(target.y, (screenBoundaries.y + player.transform.localScale.y) / 2.0f);

        target.z = player.transform.position.z;

        return target;
    }

    private void PerformAction()
    {
        player.GetComponent<Player>().ThrowSnowball();
    }
}
