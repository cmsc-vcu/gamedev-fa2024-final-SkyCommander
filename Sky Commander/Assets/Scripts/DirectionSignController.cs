using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectionSignController : MonoBehaviour
{
    public TextMeshProUGUI leftDirectionText;
    public TextMeshProUGUI rightDirectionText;
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
        attempts = 0;
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

        leftDirectionText.text = "Left: " + leftCurrentDirection;
        rightDirectionText.text = "Right: " + rightCurrentDirection;
    }

    void CheckPlayerInput()
    {
        bool isLeftCorrect = false;
        bool isRightCorrect = false;

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
            difficultyLevel++;
            GenerateNewDirections();
        }
        else if (Input.anyKeyDown)
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
        isCrashed = true;
        airplaneController.enabled = false; // Disable plane controls
        trafficLightController.enabled = false; // Disable traffic light controls
        leftDirectionText.text = "Crash!";
        rightDirectionText.text = "Crash!";

        crashPanel.SetActive(true); // Show crash panel
    }

    void RestartGame()
    {
        // Reset game variables and UI
        isCrashed = false;
        difficultyLevel = 1;
        attempts = 0;  // Reset attempts counter to 0
        airplaneController.enabled = true; // Enable plane controls
        trafficLightController.enabled = true; // Enable traffic light controls
        crashPanel.SetActive(false); // Hide crash panel
        airplaneController.ResetPlaneSize(); // Reset the plane size
        GenerateNewDirections(); // Generate new directions for the next round
    }

}
