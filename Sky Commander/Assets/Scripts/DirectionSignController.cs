using UnityEngine;
using UnityEngine.UI;

public class DirectionSignController : MonoBehaviour
{
    public Image leftDirectionImage;
    public Image rightDirectionImage;
    public Sprite upArrowSprite;
    public Sprite downArrowSprite;
    public Sprite leftArrowSprite;
    public Sprite rightArrowSprite;
    public Sprite incorrectInputSprite;
    public TrafficLightController trafficLightController;
    public AirplaneController airplaneController;
    public GameObject crashPanel;
    public Button restartButton;

    private bool leftInputLocked = false; // Tracks if the left side input is locked
    private bool rightInputLocked = false; // Tracks if the right side input is locked
    private string lockedLeftInput = ""; // Stores the locked left input direction
    private string lockedRightInput = ""; // Stores the locked right input direction
    private string leftCurrentDirection;
    private string rightCurrentDirection;
    private int successfulInputs = 0; // Number of successful inputs for landing
    private int attempts = 0; // Number of incorrect attempts
    private int maxAttempts = 3; // Max incorrect attempts before crash
    private int difficultyLevel = 1; // Difficulty level for randomization
    private bool controlsLocked = false; // Lock controls during incorrect input
    private bool isCrashed = false; // Flag for crash state
    private bool isLandingSequenceActive = false; // Prevent inputs during landing

    void Start()
    {
        GenerateNewDirections();
        crashPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (!isCrashed && !isLandingSequenceActive && !controlsLocked)
        {
            CheckPlayerInput();
        }
    }

    void GenerateNewDirections()
    {
        string[] directions = { "Up", "Down", "Left", "Right" };

        leftCurrentDirection = directions[Random.Range(0, directions.Length)];
        if (difficultyLevel == 1)
        {
            rightCurrentDirection = leftCurrentDirection; // Same direction for both
        }
        else if (difficultyLevel < 5)
        {
            rightCurrentDirection = Random.Range(0, 2) == 0 ? leftCurrentDirection : directions[Random.Range(0, directions.Length)];
        }
        else
        {
            rightCurrentDirection = directions[Random.Range(0, directions.Length)];
        }

        SetDirectionImage(leftDirectionImage, leftCurrentDirection);
        SetDirectionImage(rightDirectionImage, rightCurrentDirection);

        Debug.Log($"New directions: Left ({leftCurrentDirection}), Right ({rightCurrentDirection})");
    }

    void SetDirectionImage(Image directionImage, string direction)
    {
        switch (direction)
        {
            case "Up":
                directionImage.sprite = upArrowSprite;
                break;
            case "Down":
                directionImage.sprite = downArrowSprite;
                break;
            case "Left":
                directionImage.sprite = leftArrowSprite;
                break;
            case "Right":
                directionImage.sprite = rightArrowSprite;
                break;
        }
    }
    void LockLeftInput(string input)
    {
        leftInputLocked = true;
        lockedLeftInput = input;
        Debug.Log($"Left input locked: {input}");
    }

    void LockRightInput(string input)
    {
        rightInputLocked = true;
        lockedRightInput = input;
        Debug.Log($"Right input locked: {input}");
    }

    void ValidateInputs()
    {
        string leftMappedDirection = GetDirectionForKey(lockedLeftInput);
        string rightMappedDirection = GetDirectionForKey(lockedRightInput);

        Debug.Log($"Validating: Left ({lockedLeftInput} -> {leftMappedDirection}), Right ({lockedRightInput} -> {rightMappedDirection})");

        if (leftMappedDirection == leftCurrentDirection && rightMappedDirection == rightCurrentDirection)
        {
            Debug.Log("Inputs are correct!");
            successfulInputs++;
            ResetInputLocks();

            if (successfulInputs >= 3)
            {
                TriggerLanding();
            }
            else
            {
                GenerateNewDirections();
            }
        }
        else
        {
            Debug.Log($"Incorrect inputs: Left ({lockedLeftInput} -> {leftMappedDirection}), Right ({lockedRightInput} -> {rightMappedDirection})");
            attempts++;

            if (attempts >= maxAttempts)
            {
                TriggerCrash();
            }
            else
            {
                LockControlsAndShowIncorrectInput();
            }
        }
    }



    void ResetInputLocks()
    {
        leftInputLocked = false;
        rightInputLocked = false;
        lockedLeftInput = "";
        lockedRightInput = "";
    }

    string GetDirectionForKey(string key)
    {
        switch (key)
        {
            case "W": return "Up";
            case "S": return "Down";
            case "A": return "Left";
            case "D": return "Right";
            case "UpArrow": return "Up";
            case "DownArrow": return "Down";
            case "LeftArrow": return "Left";
            case "RightArrow": return "Right";
            default:
                Debug.LogError($"Unrecognized key: {key}");
                return ""; // Return empty if the key is unrecognized
        }
    }



    void CheckPlayerInput()
    {
        if (controlsLocked) return;

        if (!leftInputLocked)
        {
            if (Input.GetKeyDown(KeyCode.W)) LockLeftInput("W");
            else if (Input.GetKeyDown(KeyCode.S)) LockLeftInput("S");
            else if (Input.GetKeyDown(KeyCode.A)) LockLeftInput("A");
            else if (Input.GetKeyDown(KeyCode.D)) LockLeftInput("D");
        }

        if (!rightInputLocked)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) LockRightInput("UpArrow");
            else if (Input.GetKeyDown(KeyCode.DownArrow)) LockRightInput("DownArrow");
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) LockRightInput("LeftArrow");
            else if (Input.GetKeyDown(KeyCode.RightArrow)) LockRightInput("RightArrow");
        }

        if (leftInputLocked && rightInputLocked)
        {
            ValidateInputs();
        }
    }



    void LockControlsAndShowIncorrectInput()
    {
        controlsLocked = true; // Lock controls
        leftDirectionImage.sprite = incorrectInputSprite;
        rightDirectionImage.sprite = incorrectInputSprite;

        // Display "X" for 0.5 seconds instead of 1 second
        Invoke(nameof(UnlockControlsAndGenerateNewDirection), 0.5f);
    }


    void UnlockControlsAndGenerateNewDirection()
    {
        controlsLocked = false; // Unlock controls
        GenerateNewDirections();
    }

    void TriggerLanding()
    {
        isLandingSequenceActive = true;
        airplaneController.StopGrowth();
        airplaneController.PlayLandingSequence();

        // Increment difficulty level after landing
        difficultyLevel++;
        Debug.Log($"Landing successful. Difficulty level increased to {difficultyLevel}");

        // Schedule new plane spawning
        Invoke(nameof(SpawnNewPlane), 5f);

        // Reset the crash timer
        airplaneController.ResetCrashTimer();
    }


    void SpawnNewPlane()
    {
        Debug.Log("Spawning new plane with increased difficulty...");
        successfulInputs = 0; // Reset inputs
        attempts = 0; // Reset attempts
        isLandingSequenceActive = false; // Allow input again
        airplaneController.ResetPlaneSize(); // Reset the current plane
        airplaneController.IncreaseSpeed(difficultyLevel); // Speed up the plane
        GenerateNewDirections(); // Start with new directions
    }

    public void TriggerCrash()
    {
        Debug.Log("Triggering crash...");
        isCrashed = true; // Set crash state
        trafficLightController.SetActive(false); // Disable traffic light inputs
        airplaneController.PlayCrashAnimation(); // Trigger crash animation

        crashPanel.SetActive(true); // Show crash UI
        restartButton.gameObject.SetActive(true); // Ensure restart button is visible
    }


    void RestartGame()
    {
        Debug.Log("Restarting game...");
        isCrashed = false;
        successfulInputs = 0;
        attempts = 0;
        difficultyLevel = 1; // Reset difficulty level
        controlsLocked = false;
        isLandingSequenceActive = false;
        airplaneController.ResetPlaneSize();
        crashPanel.SetActive(false);

        GenerateNewDirections(); // Generate new directions
    }


}
