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

        // 1. INPUT DETECTION
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            screenPosition = Input.mousePosition;
            hasInput = true;
        }
#endif
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
                screenPosition = touch.position;
                hasInput = true;
            }
        }

        if (!hasInput) return;

        // 2. SAFETY CHECK: Did we touch the existing ball?
        // If yes, STOP here. Do not spawn a new one. Let the Drag script handle it.
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hitObject;

        if (Physics.Raycast(ray, out hitObject))
        {
            // If the thing we hit IS the ball we spawned...
            if (spawnedBall != null && hitObject.collider.gameObject == spawnedBall)
            {
                return; // STOP! Don't spawn.
            }
        }

        // 3. SPAWN LOGIC (Only runs if we hit the floor, not the ball)
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            // --- NEW FILTER: Ignore tables (too high) ---
            float heightDiff = Camera.main.transform.position.y - hitPose.position.y;

            // If the surface is less than 1.1m below our phone, it's a table. Ignore it.
            if (heightDiff < 1.1f) return;
            Vector3 safeSpawnPos = hitPose.position + (Vector3.up * 0.1f);

            if (spawnedBall == null)
            {
                spawnedBall = Instantiate(ballPrefab, safeSpawnPos, hitPose.rotation);
            }
            else
            {
                Destroy(spawnedBall);
                spawnedBall = Instantiate(ballPrefab, safeSpawnPos, hitPose.rotation);
            }

            // Ensure physics are reset
            Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}