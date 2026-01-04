using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FloorFilter : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private ARPlane plane;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        plane = GetComponent<ARPlane>();
    }

    void Update()
    {
        if (plane == null || plane.subsumedBy != null) return;

        // FIX: Removed the "minDistance" check entirely.
        // The floor is ALWAYS valid, even if you sit on it or the camera is at height 0.

        if (meshCollider) meshCollider.enabled = true;
        if (meshRenderer) meshRenderer.enabled = true;
    }
}