using UnityEngine;
using TMPro;

public class SquatPhysicsController : MonoBehaviour
{
    [Header("Configuration")]
    public float maxJumpForce = 500f;
    public float chargeSpeed = 2f;
    public TextMeshProUGUI statsDisplay; // Ensure this is linked!

    private Rigidbody rb;
    private float currentCharge = 0f;
    private bool isCharging = false;
    private float floorY;
    private float mass;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
        // Record where the floor is when we spawn
        floorY = transform.position.y;

        // Auto-find text if not assigned
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

    
    private void CheckForFalling()
    {
        // If ball is 2 meters below the floor (fell through)
        if (transform.position.y < floorY - 2.0f)
        {
            // Teleport back to slightly above the floor
            transform.position = new Vector3(transform.position.x, floorY + 0.5f, transform.position.z);

            // Kill all speed so it doesn't shoot down again
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void HandleSquatMechanics()
    {
        if (isCharging)
        {
            currentCharge += Time.deltaTime * chargeSpeed;
            currentCharge = Mathf.Clamp01(currentCharge);

            // Visual Squash
            float squash = Mathf.Lerp(1f, 0.6f, currentCharge);
            float stretch = Mathf.Lerp(1f, 1.2f, currentCharge);
            transform.localScale = new Vector3(0.2f * stretch, 0.2f * squash, 0.2f * stretch);
        }
    }

    public void StartSquat()
    {
        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f) return;
        isCharging = true;
        currentCharge = 0f;
    }

    public void ReleaseJump()
    {
        if (!isCharging) return;
        isCharging = false;

        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Reset shape

        float finalForce = currentCharge * maxJumpForce;
        rb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);
    }

    private void CalculateAndDisplayPhysics()
    {
        float h = Mathf.Max(0, transform.position.y - floorY);
        float v = rb.linearVelocity.magnitude;
        float pe = mass * 9.81f * h;
        float ke = 0.5f * mass * (v * v);

        if (statsDisplay != null)
        {
            statsDisplay.text = $"Height: {h:F2} m\nPotential: {pe:F0} J\nKinetic: {ke:F0} J";
        }
    }
}