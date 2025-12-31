using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FloorFilter : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private ARPlane plane;

    // A plane must be at least this many meters BELOW the phone to count as a floor.
    public float minDistanceBelowCamera = 1.1f;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        plane = GetComponent<ARPlane>();
    }

    void Update()
    {
        // 1. SAFETY: If components are missing, stop
        if (plane == null || plane.subsumedBy != null) return;

        // 2. EDITOR OVERRIDE: 
        // If we are in the Unity Editor, ALWAYS keep the floor solid.
#if UNITY_EDITOR
        if (meshCollider) meshCollider.enabled = true;
        if (meshRenderer) meshRenderer.enabled = true;
        return; // Stop here, don't run the filter logic below
#endif

        // 3. REAL APP LOGIC (Phone Only)
        float cameraY = Camera.main.transform.position.y;
        float planeY = transform.position.y;
        float distanceDown = cameraY - planeY;

        // Is this a floor? (Distance > 1.1m)
        bool isFloor = distanceDown > minDistanceBelowCamera;

        if (meshCollider) meshCollider.enabled = isFloor;
        if (meshRenderer) meshRenderer.enabled = isFloor;
    }
}