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
        if (isCrashed) return; // Skip updates if the plane has crashed

        // Handle plane growth (removed max scale check)
        if (isGrowing)
        {
            transform.localScale += Vector3.one * growRate * Time.deltaTime;
        }

        // Handle lateral movement during landing
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
        ResetCrashTimer(1); // Reset the timer on successful landing
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
            animator.SetTrigger("Crash");
            isCrashed = true;
            isGrowing = false;
            
        }
    }

    public void ResetPlaneSize()
    {
        Debug.Log("Resetting plane size, position, and speed...");
        transform.localScale = initialScale; // Reset size
        transform.position = startPosition; // Reset position to the start
        isGrowing = true; // Enable size growth again
        isMovingLaterally = false; // Stop any lateral movement
        isCrashed = false; // Reset crash state
        planeRenderer.flipX = false; // Reset sprite orientation

        // Reset animation state
        if (animator != null)
        {
            animator.ResetTrigger("Crash");
            animator.ResetTrigger("Land");
            animator.Play("Fly", 0, 0); // Return to the "Fly" animation state
        }

        landingSpeed = 1f; // Reset lateral movement speed
        growRate = 0.1f; // Reset growth rate
    }


    private void TriggerCrash()
    {
        if (isCrashed) return;
        Debug.Log("Plane crashed due to timeout!");
        timerActive = false; // Stop the timer
        //PlayCrashAnimation();
        // Notify the game (e.g., DirectionSignController) about the crash
        FindObjectOfType<DirectionSignController>()?.TriggerCrash();
    }
    public void IncreaseSpeed(int difficulty)
    {
        growRate += difficulty * 0.1f; // Adjust growth rate based on difficulty
        landingSpeed += difficulty * 0.5f; // Adjust lateral movement speed
    }

    public void ResetCrashTimer(int difficultyLevel)
    {
        float baseTime = 15f; // Starting time for the crash timer
        float timeReductionPerLevel = 1f; // Reduce time by 1 second per difficulty level

        // Calculate the new timer value, ensuring it doesn't drop below a minimum threshold
        crashTimer = Mathf.Max(baseTime - (difficultyLevel - 1) * timeReductionPerLevel, 5f); // Minimum of 5 seconds
        timerActive = true; // Reactivate the timer

        Debug.Log($"Crash timer reset to {crashTimer} seconds for difficulty level {difficultyLevel}");
    }

}
