using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public float growRate = 0.1f;
    public Vector3 maxScale = new Vector3(3f, 3f, 1f);
    public Vector3 startPosition;
    public SpriteRenderer planeRenderer;
    public GameObject planePrefab;
    private Animator animator;
    private Vector3 initialScale;
    private bool isGrowing = true;
    private bool isCrashed = false;
    private bool isMovingLaterally = false;
    private float landingSpeed = 1f;
    private bool moveRight = false;

    private float crashTimer = 15f; // 15-second crash timer
    private bool timerActive = true; // Flag to track timer state

    void Start()
    {
        initialScale = transform.localScale;
        startPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isCrashed) return;

        if (isGrowing && transform.localScale.x < maxScale.x)
        {
            transform.localScale += Vector3.one * growRate * Time.deltaTime;
        }

        if (isMovingLaterally)
        {
            MoveDuringLanding();
        }

        if (timerActive)
        {
            crashTimer -= Time.deltaTime;
            if (crashTimer <= 0f)
            {
                TriggerCrash();
            }
        }
    }

    public void StopGrowth()
    {
        isGrowing = false;
    }

    public void PlayLandingSequence()
    {
        Debug.Log("Playing landing animation...");
        animator.SetTrigger("Land");
        isMovingLaterally = true;
        moveRight = Random.value < 0.5f;
        planeRenderer.flipX = moveRight;
        ResetCrashTimer(); // Reset the timer on successful landing
    }

    void MoveDuringLanding()
    {
        transform.Translate(Vector3.right * (moveRight ? landingSpeed : -landingSpeed) * Time.deltaTime);
        if (Mathf.Abs(transform.position.x) > Camera.main.orthographicSize * Camera.main.aspect + 1f)
        {
            isMovingLaterally = false;
            Debug.Log("Plane exited the screen.");
        }
    }

    public void PlayCrashAnimation()
    {
        if (!isCrashed)
        {
            Debug.Log("Playing crash animation...");
            isCrashed = true;
            isGrowing = false;
            animator.SetTrigger("Crash");
        }
    }

    public void ResetPlaneSize()
    {
        Debug.Log("Resetting plane size and position...");
        transform.localScale = initialScale;
        transform.position = startPosition;
        isGrowing = true;
        isMovingLaterally = false;
        isCrashed = false;
        planeRenderer.flipX = false;

        if (animator != null)
        {
            animator.ResetTrigger("Crash");
            animator.ResetTrigger("Land");
            animator.Play("Fly", 0, 0);
        }
        ResetCrashTimer();
    }

    private void TriggerCrash()
    {
        Debug.Log("Plane crashed due to timeout!");
        isCrashed = true;
        timerActive = false; // Stop the timer
        PlayCrashAnimation();
        // Notify the game (e.g., DirectionSignController) about the crash
        FindObjectOfType<DirectionSignController>()?.TriggerCrash();
    }
    public void IncreaseSpeed(int difficulty)
    {
        growRate += difficulty * 0.1f; // Adjust growth rate based on difficulty
        landingSpeed += difficulty * 0.5f; // Adjust lateral movement speed
    }

    public void ResetCrashTimer()
    {
        crashTimer = 15f; // Reset to 15 seconds
        timerActive = true; // Reactivate the timer
    }
}
