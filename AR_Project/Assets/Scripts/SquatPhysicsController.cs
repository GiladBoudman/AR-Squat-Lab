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

    // Flag: "Has the user touched me?" (Used to detach from AR Spawner)
    public bool isBeingHeld = false;

    [Header("Dragging Settings")]
    private Vector3 mOffset;
    private float mZCoord;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        mass = rb.mass;

        // Initial floor guess
        floorY = transform.position.y;

        if (statsDisplay == null)
        {
            GameObject textObj = GameObject.Find("StatsText");
            if (textObj != null) statsDisplay = textObj.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        HandleSquatMechanics();
        CalculateAndDisplayPhysics();
        CheckForFalling();
    }

    void OnCollisionEnter(Collision collision)
    {
        // When we land on the floor/table after a drop
        if (rb.useGravity == true && !isCharging)
        {
            // Only update floor if we stopped moving vertically
            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                floorY = transform.position.y;
            }
        }
    }

    void OnMouseDown()
    {
        // 1. Pick up the ball (Gravity OFF, Physics Frozen)
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. Detach from AR Spawner
        isBeingHeld = true;

        // 3. Visuals
        if (ballRenderer != null)
            ballRenderer.material.color = Color.green;

        // 4. Dragging Math
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    void OnMouseUp()
    {
        // 1. Visual Reset
        if (ballRenderer != null)
            ballRenderer.material.color = Color.white;

        // --- NEW FEATURE: AUTO-DROP ---
        // As soon as you let go, gravity turns ON.
        rb.useGravity = true;
        rb.isKinematic = false;

        // Safety: Ensure it falls straight down (doesn't fly off sideways)
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnMouseDrag()
    {
        // Only allow dragging if we are holding it (Gravity is OFF)
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
            // Reset position if it falls into the void
            transform.position = new Vector3(transform.position.x, floorY + 0.5f, transform.position.z);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Ensure gravity is on so it falls to the floor again
            rb.useGravity = true;
            rb.isKinematic = false;
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
        // FALLBACK: If the user scans but NEVER touches the ball, 
        // this button will still drop it for them.
        if (rb.isKinematic == true)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            return;
        }

        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f) return;
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
            statsDisplay.text =
                $"<b>Height (h):</b> {h:F2} m\n\n" +
                $"<b>PE = m·g·h</b>\n" +
                $"{mass} · 9.81 · {h:F2} = <color=yellow><b>{pe:F0} J</b></color>\n\n" +
                $"<b>KE = ½·m·v²</b>\n" +
                $"0.5 · {mass} · {v:F1}² = <color=yellow><b>{ke:F0} J</b></color>";
        }
    }
}