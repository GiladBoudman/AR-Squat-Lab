using UnityEngine;
using TMPro;

public class SquatPhysicsController : MonoBehaviour
{
    [Header("Configuration")]
    public float maxJumpForce = 500f;
    public float chargeSpeed = 2f;
    public TextMeshProUGUI statsDisplay;

    private Rigidbody rb;
    private Renderer ballRenderer;
    private float currentCharge = 0f;
    private bool isCharging = false;
    private float floorY;
    private float mass;

    // --- NEW: MAX HEIGHT MARKER ---
    private GameObject highPointMarker;
    // ------------------------------

    public bool isBeingHeld = false;

    [Header("Dragging Settings")]
    private Vector3 mOffset;
    private float mZCoord;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        mass = rb.mass;
        floorY = transform.position.y;

        if (statsDisplay == null)
        {
            GameObject textObj = GameObject.Find("StatsText");
            if (textObj != null) statsDisplay = textObj.GetComponent<TextMeshProUGUI>();
        }

        // --- NEW: CREATE THE MARKER AUTOMATICALLY ---
        CreateHighPointMarker();
    }

    // Creates a thin red disk to mark the highest point
    void CreateHighPointMarker()
    {
        highPointMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(highPointMarker.GetComponent<Collider>()); // Remove physics so ball doesn't hit it

        // Make it a thin disk
        highPointMarker.transform.localScale = new Vector3(0.4f, 0.01f, 0.4f);

        // Make it Red/Transparent
        Renderer r = highPointMarker.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Standard")); // Standard shader supports transparency
        r.material.color = new Color(1f, 0f, 0f, 0.5f); // Red with 50% transparency

        // Hide it initially
        highPointMarker.SetActive(false);
    }

    void Update()
    {
        HandleSquatMechanics();
        CalculateAndDisplayPhysics();
        CheckForFalling();
        UpdateMaxHeightMarker(); // <--- Check height every frame
    }

    // --- NEW: PUSH THE MARKER UP ---
    void UpdateMaxHeightMarker()
    {
        if (highPointMarker == null || !highPointMarker.activeSelf) return;

        // Keep the marker aligned with the ball horizontally (X/Z)
        Vector3 currentMarkerPos = highPointMarker.transform.position;
        currentMarkerPos.x = transform.position.x;
        currentMarkerPos.z = transform.position.z;

        // Only push the marker UP (Y), never down
        if (transform.position.y > currentMarkerPos.y)
        {
            currentMarkerPos.y = transform.position.y;
        }

        highPointMarker.transform.position = currentMarkerPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb.useGravity == true && !isCharging)
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                floorY = transform.position.y;
            }
        }
    }

    void OnMouseDown()
    {
        // RESET MARKER when grabbing
        if (highPointMarker != null) highPointMarker.SetActive(false);

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isBeingHeld = true;

        if (ballRenderer != null)
            ballRenderer.material.color = Color.green;

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    void OnMouseUp()
    {
        if (ballRenderer != null)
            ballRenderer.material.color = Color.white;

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnMouseDrag()
    {
        if (rb.useGravity == false)
        {
            transform.position = GetMouseAsWorldPoint() + mOffset;
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void CheckForFalling()
    {
        if (transform.position.y < floorY - 2.0f)
        {
            transform.position = new Vector3(transform.position.x, floorY + 0.5f, transform.position.z);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = false;

            // Hide marker if we fell
            if (highPointMarker != null) highPointMarker.SetActive(false);
        }
    }

    private void HandleSquatMechanics()
    {
        if (isCharging)
        {
            currentCharge += Time.deltaTime * chargeSpeed;
            currentCharge = Mathf.Clamp01(currentCharge);

            float squash = Mathf.Lerp(1f, 0.6f, currentCharge);
            float stretch = Mathf.Lerp(1f, 1.2f, currentCharge);
            transform.localScale = new Vector3(0.2f * stretch, 0.2f * squash, 0.2f * stretch);
        }
    }

    public void StartSquat()
    {
        if (rb.isKinematic == true)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            return;
        }

        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f) return;

        // --- RESET MARKER ON SQUAT START ---
        if (highPointMarker != null)
        {
            highPointMarker.SetActive(true);
            // Start the marker exactly at the ball's center
            highPointMarker.transform.position = transform.position;
        }
        // -----------------------------------

        isCharging = true;
        currentCharge = 0f;
    }

    public void ReleaseJump()
    {
        if (!isCharging) return;
        isCharging = false;

        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        float finalForce = currentCharge * maxJumpForce;
        rb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);
    }

    private void CalculateAndDisplayPhysics()
    {
        float h = Mathf.Max(0, transform.position.y - floorY);
        if (h < 0.01f) h = 0f;

        float v = rb.linearVelocity.magnitude;
        float pe = mass * 9.81f * h;
        float ke = 0.5f * mass * (v * v);

        if (statsDisplay != null)
        {
            // Calculate Max Height for display
            float maxH = 0f;
            if (highPointMarker != null && highPointMarker.activeSelf)
            {
                maxH = Mathf.Max(0, highPointMarker.transform.position.y - floorY);
            }

            statsDisplay.text =
                $"<b>Height:</b> {h:F2} m <color=red>(Max: {maxH:F2})</color>\n\n" +
                $"<b>PE = m·g·h</b>\n" +
                $"{mass} · 9.81 · {h:F2} = <color=yellow><b>{pe:F0} J</b></color>\n\n" +
                $"<b>KE = ½·m·v²</b>\n" +
                $"0.5 · {mass} · {v:F1}² = <color=yellow><b>{ke:F0} J</b></color>";
        }
    }

    // --- QUIZ HELPERS ---
    public float GetMaxHeight()
    {
        if (highPointMarker != null && highPointMarker.activeSelf)
        {
            return Mathf.Max(0, highPointMarker.transform.position.y - floorY);
        }
        return 0f;
    }

    public void ResetMarker()
    {
        if (highPointMarker != null) highPointMarker.SetActive(false);
    }
}