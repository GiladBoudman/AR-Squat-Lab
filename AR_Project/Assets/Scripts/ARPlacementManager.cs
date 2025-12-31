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
        // Mouse Input for Editor
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            screenPosition = Input.mousePosition;
            hasInput = true;
        }
#endif
        // Touch Input for Phone
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

        // 2. SAFETY CHECK: Did we hit the existing ball?
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            // If we hit the ball, STOP. Do not spawn a new one.
            if (spawnedBall != null && hitObject.collider.gameObject == spawnedBall)
            {
                return;
            }
        }

        // 3. SPAWN LOGIC (AR Raycast)
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // --- FILTER LOGIC (Disable in Editor so you can test) ---
#if !UNITY_EDITOR
            // Only run this check on the actual phone
            float heightDiff = Camera.main.transform.position.y - hitPose.position.y;
            if (heightDiff < 1.1f) return; // Block tables
#endif
            // --------------------------------------------------------

            Vector3 safeSpawnPos = hitPose.position + (Vector3.up * 0.05f);

            if (spawnedBall == null)
            {
                spawnedBall = Instantiate(ballPrefab, safeSpawnPos, hitPose.rotation);
            }
            else
            {
                spawnedBall.transform.position = safeSpawnPos;
                spawnedBall.transform.rotation = hitPose.rotation;
            }

            // FORCE FLOATING MODE (So you can drag it immediately)
            Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        // 4. EDITOR FALLBACK (If you are testing without XR Simulation)
#if UNITY_EDITOR
        else
        {
            // If AR Raycast fails (common in Editor), try a standard Physics Raycast
            if (Physics.Raycast(ray, out RaycastHit fallbackHit))
            {
                // Only spawn if we hit something that looks like a floor (facing up)
                if (fallbackHit.normal == Vector3.up)
                {
                    if (spawnedBall == null)
                        spawnedBall = Instantiate(ballPrefab, fallbackHit.point + Vector3.up * 0.05f, Quaternion.identity);
                    else
                        spawnedBall.transform.position = fallbackHit.point + Vector3.up * 0.05f;

                    // Ensure Floating
                    Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
        }
#endif
    }
}