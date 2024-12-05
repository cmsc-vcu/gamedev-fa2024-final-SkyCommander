using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    // References to the left and right light pivot objects
    public GameObject leftLightPivot;
    public GameObject rightLightPivot;

    // Define rotation angles for each direction
    private Vector3 upRotation = new Vector3(0, 0, 0);     // Up
    private Vector3 downLRotation = new Vector3(0, 0, 180 - 45); // Down
    private Vector3 downRRotation = new Vector3(0, 0, 180 + 50); // Down
    private Vector3 leftRotation = new Vector3(0, 0, 45);  // Left
    private Vector3 rightRotation = new Vector3(0, 0, -45); // Right

    private bool isActive = true; // Flag to enable/disable input handling

    void Update()
    {
        if (isActive)
        {
            HandleLeftLightInput();
            HandleRightLightInput();
        }
    }

    void HandleLeftLightInput()
    {
        // Rotate left light pivot based on WASD input
        if (Input.GetKey(KeyCode.W))
        {
            leftLightPivot.transform.localEulerAngles = upRotation;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            leftLightPivot.transform.localEulerAngles = downLRotation;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            leftLightPivot.transform.localEulerAngles = leftRotation;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            leftLightPivot.transform.localEulerAngles = rightRotation;
        }
    }

    void HandleRightLightInput()
    {
        // Rotate right light pivot based on arrow keys input
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rightLightPivot.transform.localEulerAngles = upRotation;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rightLightPivot.transform.localEulerAngles = downRRotation;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            rightLightPivot.transform.localEulerAngles = leftRotation;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rightLightPivot.transform.localEulerAngles = rightRotation;
        }
    }

    // Method to reset lights to their default rotation
    public void ResetLights()
    {
        leftLightPivot.transform.localEulerAngles = upRotation;
        rightLightPivot.transform.localEulerAngles = upRotation;
    }

    // Method to enable or disable input handling
    public void SetActive(bool active)
    {
        isActive = active;
    }
}
