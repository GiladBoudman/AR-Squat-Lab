using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageToPhysicsSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public ARTrackedImageManager imageManager;
    public GameObject ballPrefab;

    [Header("Position Adjustment (Meters)")]
    // X = Right/Left, Y = Out (Towards you), Z = Up/Down
    public Vector3 spawnOffset = new Vector3(0f, 0.2f, 0f);

    [Header("State")]
    public GameObject spawnedBall;

    void OnEnable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.AddListener(OnChanged);
    }

    void OnDisable()
    {
        if (imageManager != null)
            imageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // 1. Handle NEW images
        foreach (var newImage in eventArgs.added)
        {
            UpdateBall(newImage);
        }

        // 2. Handle UPDATED images (Keep ball stuck to image if moving)
        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                UpdateBall(updatedImage);
            }
        }
    }

    // Consolidated function to Spawn OR Update position
    void UpdateBall(ARTrackedImage image)
    {
        // 1. Create ball if missing
        if (spawnedBall == null)
        {
            spawnedBall = Instantiate(ballPrefab, image.transform.position, Quaternion.identity);
            Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.useGravity = false; rb.isKinematic = true; }
        }

        // 2. CHECK: Is the user holding the ball?
        SquatPhysicsController controller = spawnedBall.GetComponent<SquatPhysicsController>();

        // IF the user has ever touched it (isBeingHeld is true), STOP following the image.
        if (controller != null && controller.isBeingHeld)
        {
            return; // Exit! Do not move the ball automatically anymore.
        }

        // 3. If user is NOT holding it, keep snapping it to the image
        Rigidbody ballRb = spawnedBall.GetComponent<Rigidbody>();
        if (ballRb != null && ballRb.useGravity == false)
        {
            Vector3 finalPos = image.transform.position + (image.transform.rotation * spawnOffset);
            spawnedBall.transform.position = finalPos;
            spawnedBall.transform.rotation = image.transform.rotation;
        }
    }
}