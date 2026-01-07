using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// This script allows the user to place a ball prefab on detected AR planes
/// </summary>
public class ARPlacementManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject ballPrefab;
    public GameObject spawnedBall;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        Vector2 inputPosition = Vector2.zero;
        bool hasInput = false;

        // Detect Input (Touch or Mouse)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            hasInput = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputPosition = Mouse.current.position.ReadValue();
            hasInput = true;
        }

        if (!hasInput) return;

        // Check UI Blocking 
        if (IsPointerOverUI(inputPosition)) return;

        // AR Raycast to place the ball
        if (raycastManager.Raycast(inputPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;


            // This prevents it from spawning underground and popping up high.
            Vector3 safeSpawnPos = hitPose.position + Vector3.up * 0.12f;

            if (spawnedBall == null)
            {
                spawnedBall = Instantiate(ballPrefab, safeSpawnPos, hitPose.rotation);
            }
            else
            {
                spawnedBall.transform.position = safeSpawnPos;
            }

            // Make sure physics is correctly reset
            Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
    }

    // Check if the pointer is over a UI element
    private bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult r in results)
        {
            if (r.gameObject.layer == 5)
            {
                return true;
            }
        }
        return false;
    }
}