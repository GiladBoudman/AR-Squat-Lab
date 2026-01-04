using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class SquatPhysicsController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
    private GameObject highPointMarker;
    public bool isBeingHeld = false;
    private float distanceFromCamera;

    // TRACKING STATE
    private bool initialFloorFound = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        mass = rb.mass;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        rb.useGravity = false;
        rb.isKinematic = true;

        floorY = transform.position.y;

        if (statsDisplay == null)
        {
            GameObject textObj = GameObject.Find("StatsText");
            if (textObj != null) statsDisplay = textObj.GetComponent<TextMeshProUGUI>();
        }

        CreateHighPointMarker();
    }

    void Update()
    {
        HandleSquatMechanics();
        CalculateAndDisplayPhysics();
        CheckForFalling();
        UpdateMaxHeightMarker();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // First landing calibration
        if (!initialFloorFound)
        {
            floorY = transform.position.y;

            if (highPointMarker != null)
            {
                Vector3 p = highPointMarker.transform.position;
                p.y = floorY;
                highPointMarker.transform.position = p;
            }
            initialFloorFound = true;
        }
    }

    // --- 1. UI BUTTON LOGIC ---

    public void StartSquat()
    {
        if (isBeingHeld) return;

        // RESET MARKER FOR NEW JUMP
        if (highPointMarker != null)
        {
            highPointMarker.SetActive(true);

            // Teleport to ball to start fresh
            Vector3 resetPos = transform.position;
            if (initialFloorFound) resetPos.y = floorY;
            highPointMarker.transform.position = resetPos;
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        isCharging = true;
        currentCharge = 0f;
    }

    public void ReleaseJump()
    {
        if (isCharging) PerformJump();
    }

    private void PerformJump()
    {
        isCharging = false;
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        rb.isKinematic = false;
        rb.useGravity = true;

        float finalForce = currentCharge * maxJumpForce;
        rb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);
    }

    // --- 2. BALL TOUCH LOGIC ---

    public void OnPointerDown(PointerEventData eventData)
    {
        // HIDE MARKER WHEN MOVING (New jump logic)
        if (highPointMarker != null) highPointMarker.SetActive(false);

        isCharging = false;
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        transform.rotation = Quaternion.identity;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        isBeingHeld = true;
        if (ballRenderer != null) ballRenderer.material.color = Color.green;

        distanceFromCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ballRenderer != null) ballRenderer.material.color = Color.green;
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            Vector3 newPos = ray.GetPoint(distanceFromCamera);
            transform.position = newPos;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ballRenderer != null) ballRenderer.material.color = Color.white;
        isBeingHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    // --- MECHANICS ---

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

    private void CalculateAndDisplayPhysics()
    {
        float h = Mathf.Max(0, transform.position.y - floorY);
        if (h < 0.01f) h = 0f;

        float v = rb.linearVelocity.magnitude;
        float pe = mass * 9.81f * h;
        float ke = 0.5f * mass * (v * v);

        if (statsDisplay != null)
        {
            float maxH = GetMaxHeight();
            statsDisplay.text =
                $"<b>Height:</b> {h:F2} m <color=red>(Max: {maxH:F2})</color>\n" +
                $"<b>PE = m·g·h</b> = " +
                $"{mass}·9.81·{h:F2} = <color=yellow><b>{pe:F0} J</b></color>\n" +
                $"<b>KE = ½·m·v²</b> = " +
                $"0.5·{mass}·{v:F1}² = <color=yellow><b>{ke:F0} J</b></color>";
        }
    }

    void UpdateMaxHeightMarker()
    {
        if (highPointMarker == null) return;

        // Horizontal sync
        Vector3 p = highPointMarker.transform.position;
        p.x = transform.position.x;
        p.z = transform.position.z;

        // Vertical Logic
        if (!initialFloorFound)
        {
            p.y = transform.position.y; // Follow down
        }
        else
        {
            // Only go UP
            if (transform.position.y > p.y) p.y = transform.position.y;
        }

        highPointMarker.transform.position = p;
    }

    private void CheckForFalling()
    {
        if (transform.position.y < floorY - 5.0f)
        {
            transform.position = new Vector3(transform.position.x, floorY + 1.0f, transform.position.z);
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
            initialFloorFound = false;
        }
    }

    public float GetMaxHeight() { return highPointMarker != null ? Mathf.Max(0, highPointMarker.transform.position.y - floorY) : 0f; }
    public void ResetMarker() { if (highPointMarker != null) highPointMarker.SetActive(false); }

    void CreateHighPointMarker()
    {
        highPointMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(highPointMarker.GetComponent<Collider>());
        highPointMarker.transform.localScale = new Vector3(0.3f, 0.01f, 0.3f);

        Renderer r = highPointMarker.GetComponent<Renderer>();

        // --- TRANSPARENCY SETUP ---
        // Create a new material using Standard Shader
        Material mat = new Material(Shader.Find("Standard"));

        // Force the material into "Fade" or "Transparent" mode
        mat.SetFloat("_Mode", 3); // 3 = Transparent
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        // Set Color with low Alpha (0.15f is very transparent)
        mat.color = new Color(1f, 0f, 0f, 0.15f);

        r.material = mat;

        highPointMarker.SetActive(false);
    }
}