using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This component hides all horizontal planes that are not the lowest detected floor
/// </summary>
public class ARFloorCleaner : MonoBehaviour
{
    private ARPlaneManager planeManager;
    private float heightTolerance = 0.05f; // 5cm tolerance

    void Awake()
    {
        // Automatically find the manager on this object
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (planeManager == null) return;

        float lowestY = float.MaxValue;

        // find the real wall (lowest y) 
        foreach (var plane in planeManager.trackables)
        {
            // Only check horizontal floors, ignore walls and ceilings
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                if (plane.transform.position.y < lowestY)
                {
                    lowestY = plane.transform.position.y;
                }
            }
        }

        // Hide ghost planes that are higher than the real floor
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                // If a plane is higher than (floor + 5cm) hide it
                bool isRealFloor = plane.transform.position.y < (lowestY + heightTolerance);

                // Only change active state if needed
                if (plane.gameObject.activeSelf != isRealFloor)
                {
                    plane.gameObject.SetActive(isRealFloor);
                }
            }
        }
    }
}