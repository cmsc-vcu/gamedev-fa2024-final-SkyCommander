using UnityEngine;
using TMPro;

public class DirectionSignController : MonoBehaviour
{
    public TextMeshProUGUI leftDirectionText;
    public TextMeshProUGUI rightDirectionText;
    public TrafficLightController trafficLightController; // Reference to the light controller
    public AirplaneController airplaneController; // Reference to the airplane controller for crash behavior

    private string leftCurrentDirection; // Current direction for the left lightstick
    private string rightCurrentDirection; // Current direction for the right lightstick
    private int attempts = 0; // Number of attempts the player has made
    private int maxAttempts = 3; // Maximum allowed attempts
    private int difficultyLevel = 1; // Difficulty level, starts at 1

    void Start()
    {
        GenerateNewDirections();
    }

    void Update()
    {
        CheckPlayerInput();
    }

    void GenerateNewDirections()
    {
        // Reset attempts
        attempts = 0;

        // Generate random directions for left and right lightsticks based on difficulty
        string[] directions = { "Up", "Down", "Left", "Right" };
        leftCurrentDirection = directions[Random.Range(0, directions.Length)];

        // Increase randomness as the difficulty level rises
        if (difficultyLevel < 3) // Easy difficulty: both directions are the same
        {
            rightCurrentDirection = leftCurrentDirection;
        }
        else if (difficultyLevel < 5) // Medium difficulty: 50% chance of both directions being the same
        {
            rightCurrentDirection = Random.Range(0, 2) == 0 ? leftCurrentDirection : directions[Random.Range(0, directions.Length)];
        }
        else // Hard difficulty: fully random directions for left and right
        {
            rightCurrentDirection = directions[Random.Range(0, directions.Length)];
        }

        // Update the direction text to show the player
        leftDirectionText.text = "Left: " + leftCurrentDirection;
        rightDirectionText.text = "Right: " + rightCurrentDirection;
    }

    void CheckPlayerInput()
    {
        bool isLeftCorrect = false;
        bool isRightCorrect = false;

        // Check if the left lightstick matches the left direction
        switch (leftCurrentDirection)
        {
            case "Up":
                isLeftCorrect = Input.GetKey(KeyCode.W);
                break;
            case "Down":
                isLeftCorrect = Input.GetKey(KeyCode.S);
                break;
            case "Left":
                isLeftCorrect = Input.GetKey(KeyCode.A);
                break;
            case "Right":
                isLeftCorrect = Input.GetKey(KeyCode.D);
                break;
        }

        // Check if the right lightstick matches the right direction
        switch (rightCurrentDirection)
        {
            case "Up":
                isRightCorrect = Input.GetKey(KeyCode.UpArrow);
                break;
            case "Down":
                isRightCorrect = Input.GetKey(KeyCode.DownArrow);
                break;
            case "Left":
                isRightCorrect = Input.GetKey(KeyCode.LeftArrow);
                break;
            case "Right":
                isRightCorrect = Input.GetKey(KeyCode.RightArrow);
                break;
        }

        if (isLeftCorrect && isRightCorrect)
        {
            // Both directions are correct, so generate new directions and increase difficulty
            difficultyLevel++; // Increase difficulty level
            GenerateNewDirections();
        }
        else if (Input.anyKeyDown) // Only count incorrect input on key press
        {
            attempts++;
            if (attempts >= maxAttempts)
            {
                TriggerCrash();
            }
        }
    }

    void TriggerCrash()
    {
        // Logic for what happens on crash (e.g., stop airplane growth or display crash message)
        airplaneController.enabled = false; // Stop the airplane from growing as an example
        leftDirectionText.text = "Crash!"; // Display crash message on the left
        rightDirectionText.text = "Crash!"; // Display crash message on the right

        // Optionally reset difficulty level on crash
        difficultyLevel = 1;
    }
}
