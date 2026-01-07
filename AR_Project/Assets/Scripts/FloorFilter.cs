using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// This component ensures that the floor plane remains visible and collidable
/// </summary>
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
        if (meshCollider) meshCollider.enabled = true;
        if (meshRenderer) meshRenderer.enabled = true;
    }
}