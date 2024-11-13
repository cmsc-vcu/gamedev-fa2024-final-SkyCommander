using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public float growRate = 0.1f;
    public Vector3 maxScale = new Vector3(3f, 3f, 1f);
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale; // Save initial size of the plane
    }

    void Update()
    {
        if (transform.localScale.x < maxScale.x && transform.localScale.y < maxScale.y)
        {
            transform.localScale += Vector3.one * growRate * Time.deltaTime;
        }
    }

    public void StopGrowth()
    {
        enabled = false;
    }

    public void ResetPlaneSize()
    {
        transform.localScale = initialScale; // Reset plane size to initial
    }
}
