using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARPlacementManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject ballPrefab;
    public GameObject spawnedBall;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        Vector2 screenPosition = Vector2.zero;
        bool hasInput = false;

        // 1. MOUSE (Simulator)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // Block UI click
            screenPosition = Input.mousePosition;
            hasInput = true;
        }
#endif

        // 2. TOUCH (Phone)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return; // Block UI touch
                screenPosition = touch.position;
                hasInput = true;
            }
        }

        if (!hasInput) return;

        // 3. SPAWN LOGIC
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // FIX: Spawn 10cm UP so it doesn't clip into the floor
            Vector3 safeSpawnPos = hitPose.position + (Vector3.up * 0.1f);

            if (spawnedBall == null)
            {
                spawnedBall = Instantiate(ballPrefab, safeSpawnPos, hitPose.rotation);
            }
            else
            {
                // Move existing ball
                spawnedBall.transform.position = safeSpawnPos;

                // FIX: Kill velocity so it doesn't carry momentum and roll away
                Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}