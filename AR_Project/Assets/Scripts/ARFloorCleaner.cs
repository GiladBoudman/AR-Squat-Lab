using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

        // 1. FIND THE REAL FLOOR (Lowest Y)
        foreach (var plane in planeManager.trackables)
        {
            // Only check horizontal floors, ignore walls
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                if (plane.transform.position.y < lowestY)
                {
                    lowestY = plane.transform.position.y;
                }
            }
        }

        // 2. HIDE THE GHOST FLOORS
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                // If a plane is higher than (Floor + 5cm), it is a ghost. Hide it.
                bool isRealFloor = plane.transform.position.y < (lowestY + heightTolerance);

                // Only change active state if needed (optimization)
                if (plane.gameObject.activeSelf != isRealFloor)
                {
                    plane.gameObject.SetActive(isRealFloor);
                }
            }
        }
    }
}