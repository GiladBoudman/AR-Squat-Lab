using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This component spawns a physics enabled ball at the position of a tracked image.
/// </summary>
public class ImageToPhysicsSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public ARTrackedImageManager imageManager;
    public GameObject ballPrefab;

    // Offset to spawn the ball slightly above the image
    [Header("Position Adjustment (Meters)")]
    public Vector3 spawnOffset = new Vector3(0f, 0.2f, 0f);

    [Header("State")]
    public GameObject spawnedBall;

    // Listen to image changes
    void OnEnable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.AddListener(OnChanged);
    }

    // Stop listening to image changes
    void OnDisable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    // Handle added/updated images
    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Handle newly detected images
        foreach (var newImage in eventArgs.added)
        {
            UpdateBall(newImage);
        }

        // Handle updated images (position changes)
        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                UpdateBall(updatedImage);
            }
        }
    }

    // Update or create the ball at the image position
    void UpdateBall(ARTrackedImage image)
    {
        // Create the ball if it doesn't exist
        if (spawnedBall == null)
        {
            spawnedBall = Instantiate(ballPrefab, image.transform.position, Quaternion.identity);
            Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.useGravity = false; rb.isKinematic = true; }
        }

        // Check if the user is holding the ball 
        SquatPhysicsController controller = spawnedBall.GetComponent<SquatPhysicsController>();

        // If user is holding the ball do not move it automatically
        if (controller != null && controller.isBeingHeld)
        {
            return; // Exit early if being held
        }

        // Position the ball above the image
        Rigidbody ballRb = spawnedBall.GetComponent<Rigidbody>();
        if (ballRb != null && ballRb.useGravity == false)
        {
            Vector3 finalPos = image.transform.position + (image.transform.rotation * spawnOffset);
            spawnedBall.transform.position = finalPos;
            spawnedBall.transform.rotation = image.transform.rotation;
        }
    }
}