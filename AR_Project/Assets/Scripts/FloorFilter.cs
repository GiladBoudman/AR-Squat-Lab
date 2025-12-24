using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FloorFilter : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private ARPlane plane;

    // A plane must be at least this many meters BELOW the phone to count as a floor.
    // Tables are usually ~0.8m below your eyes.
    // Floors are usually ~1.6m below your eyes.
    // We set the cutoff at 1.1m.
    public float minDistanceBelowCamera = 1.1f;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        plane = GetComponent<ARPlane>();
    }

    void Update()
    {
        // Safety check
        if (plane == null || plane.subsumedBy != null) return;

        // 1. Calculate height difference
        float cameraY = Camera.main.transform.position.y;
        float planeY = transform.position.y;
        float distanceDown = cameraY - planeY;

        // 2. Decide: Is this a floor?
        bool isFloor = distanceDown > minDistanceBelowCamera;

        // 3. Apply: Turn it ON if it's a floor, OFF if it's a table
        if (meshCollider) meshCollider.enabled = isFloor;
        if (meshRenderer) meshRenderer.enabled = isFloor;
    }
}