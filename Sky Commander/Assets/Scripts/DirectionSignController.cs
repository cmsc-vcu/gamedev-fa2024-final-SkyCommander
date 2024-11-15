using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private string leftCurrentDirection;
    private string rightCurrentDirection;
    private int attempts = 0;
    private int maxAttempts = 3;
    private int difficultyLevel = 1;
    private bool isCrashed = false;

    // Track if inputs were received for both sides
    private bool leftInputReceived = false;
    private bool rightInputReceived = false;
    private bool isLeftCorrect = false;
    private bool isRightCorrect = false;

    void Start()
    {
        GenerateNewDirections();
        crashPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (!isCrashed)
        {
            CheckPlayerInput();
        }
    }

    void GenerateNewDirections()
    {
        attempts = 0; // Reset attempts for each new direction
        leftInputReceived = false;
        rightInputReceived = false;

        string[] directions = { "Up", "Down", "Left", "Right" };
        leftCurrentDirection = directions[Random.Range(0, directions.Length)];

        if (difficultyLevel < 3)
        {
            rightCurrentDirection = leftCurrentDirection;
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

    void CheckPlayerInput()
    {
        // Check WASD inputs for left direction
        if (!leftInputReceived)
        {
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

            if (isLeftCorrect || Input.anyKeyDown)
            {
                leftInputReceived = true; // Mark left input as received
            }
        }

        // Check arrow key inputs for right direction
        if (!rightInputReceived)
        {
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

            if (isRightCorrect || Input.anyKeyDown)
            {
                rightInputReceived = true; // Mark right input as received
            }
        }

        // Only check for correctness once both inputs are received
        if (leftInputReceived && rightInputReceived)
        {
            if (isLeftCorrect && isRightCorrect)
            {
                difficultyLevel++;
                GenerateNewDirections();
            }
            else
            {
                attempts++;
                ShowIncorrectInput();

                if (attempts >= maxAttempts)
                {
                    TriggerCrash();
                }
            }

            // Reset inputs for the next round
            leftInputReceived = false;
            rightInputReceived = false;
            isLeftCorrect = false;
            isRightCorrect = false;
        }
    }

    void ShowIncorrectInput()
    {
        leftDirectionImage.sprite = incorrectInputSprite;
        rightDirectionImage.sprite = incorrectInputSprite;
        Invoke(nameof(GenerateNewDirections), 1.0f); // Show "X" for a moment before changing directions
    }

    void TriggerCrash()
    {
        isCrashed = true;
        airplaneController.enabled = false;
        trafficLightController.enabled = false;
        leftDirectionImage.sprite = incorrectInputSprite;
        rightDirectionImage.sprite = incorrectInputSprite;
        crashPanel.SetActive(true);
    }

    void RestartGame()
    {
        isCrashed = false;
        difficultyLevel = 1;
        attempts = 0;
        airplaneController.enabled = true;
        trafficLightController.enabled = true;
        crashPanel.SetActive(false);
        airplaneController.ResetPlaneSize();
        GenerateNewDirections();
    }
}
