using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public float growRate = 0.1f; // Rate at which the airplane grows
    public Vector3 maxScale = new Vector3(3f, 3f, 1f); // Maximum size before it stops growing

    void Update()
    {
        // Gradually increase the airplane's size
        if (transform.localScale.x < maxScale.x && transform.localScale.y < maxScale.y)
        {
            transform.localScale += Vector3.one * growRate * Time.deltaTime;
        }
    }
    public void StopGrowth()
    {
        enabled = false; // Disable the growth behavior
    }

}