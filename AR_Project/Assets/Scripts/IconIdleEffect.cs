using UnityEngine;

/// <summary>
/// This component makes an icon float, pulse, and wiggle for a lively idle effect
/// </summary>
public class IconIdleEffect : MonoBehaviour
{
    [Header("Movement Settings")]
    public float floatSpeed = 2f;       // How fast it moves up/down
    public float floatStrength = 10f;   // How far it moves 

    [Header("Pulse Settings")]
    public float pulseSpeed = 3f;       // How fast it grows/shrinks
    public float pulseStrength = 0.1f;  // How much it grows (0.1 = 10%)

    [Header("Rotation Settings")]
    public float rotateSpeed = 1f;      // How fast it wobbles
    public float rotateAngle = 5f;      // Max tilt angle

    private Vector3 originalPos;
    private Vector3 originalScale;
    private float randomOffset; // Makes each icon move differently

    void Start()
    {
        // Remember where we started so we can wobble around that point
        originalPos = transform.localPosition;
        originalScale = transform.localScale;

        // Create a random start time so icons don't move in perfect sync
        randomOffset = Random.Range(0f, 5f);
    }

    void Update()
    {
        // For each frame, we will apply 3 effects: Floating, Pulsing, and Wiggling
        float newY = originalPos.y + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatStrength;
        transform.localPosition = new Vector3(originalPos.x, newY, originalPos.z);

        float scaleChange = Mathf.Sin((Time.time + randomOffset) * pulseSpeed) * pulseStrength;
        transform.localScale = originalScale + (Vector3.one * scaleChange);

        float zRotation = Mathf.Sin((Time.time + randomOffset) * rotateSpeed) * rotateAngle;
        transform.localRotation = Quaternion.Euler(0, 0, zRotation);
    }
}