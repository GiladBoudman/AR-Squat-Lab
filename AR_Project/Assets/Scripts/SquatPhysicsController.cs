using OpenCover.Framework.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This component manages the squat-and-jump mechanics of a ball
/// </summary>
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
    private bool initialFloorFound = false;
    private bool showConservationMode = false;
    private float storedV0 = 0f;   // Initial Velocity
    private float storedEk0 = 0f;  // Initial Kinetic Energy

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

        // Auto-find text if missing
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

    public void ToggleEnergyMode()
    {
        showConservationMode = !showConservationMode;
    }

    public void StartSquat()
    {
        if (isBeingHeld) return;

        // Reset marker for new attempt
        if (highPointMarker != null)
        {
            highPointMarker.SetActive(true);
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

        // Calculate Impulse Force
        float impulse = currentCharge * maxJumpForce;

        // Apply Force
        rb.AddForce(Vector3.up * impulse, ForceMode.Impulse);

        // Impulse J = m * delta_v  =>  v = J / m
        storedV0 = impulse / mass;

        // Ek0 = 0.5 * m * v0^2
        storedEk0 = 0.5f * mass * (storedV0 * storedV0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
        if (statsDisplay == null) return;

        float h = Mathf.Max(0, transform.position.y - floorY);
        float maxH = GetMaxHeight();

        if (showConservationMode)
        {
            // MODE 2: Conservation View
            float Epf = mass * 9.81f * maxH;

            statsDisplay.text =
                $"<b>v0 (Launch):</b> {storedV0:F2} m/s\n" +
                $"<b>Ek0 (Initial):</b> <color=yellow>{storedEk0:F0} J</color>\n" +
                $"<b>Epf (Final):</b> <color=green>{Epf:F0} J</color>\n" +
                $"<size=80%>(Ek0 ≈ Epf)</size>";
        }
        else
        {
            // MODE 1: Real-Time View 
            float v = rb.linearVelocity.magnitude;
            float pe = mass * 9.81f * h;
            float ke = 0.5f * mass * (v * v);

            statsDisplay.text =
                $"<b>Height:</b> {h:F2} m <color=red>(Max: {maxH:F2})</color>\n" +

                $"<b>Potential Energy (mgh):</b> <color=yellow>{pe:F0} J</color>\n" +

                $"<b>Kinteic Energy (½mv²):</b> <color=green>{ke:F0} J</color>\n";
        }
    }

    void UpdateMaxHeightMarker()
    {
        if (highPointMarker == null) return;
        Vector3 p = highPointMarker.transform.position;
        p.x = transform.position.x;
        p.z = transform.position.z;
        if (!initialFloorFound) p.y = transform.position.y;
        else if (transform.position.y > p.y) p.y = transform.position.y;
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

    public float GetMaxHeight() => highPointMarker != null ? Mathf.Max(0, highPointMarker.transform.position.y - floorY) : 0f;
    public void ResetMarker() { if (highPointMarker != null) highPointMarker.SetActive(false); }

    // Marker Creation Code 
    void CreateHighPointMarker()
    {
        highPointMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(highPointMarker.GetComponent<Collider>());
        highPointMarker.transform.localScale = new Vector3(0.3f, 0.01f, 0.3f);

        Renderer r = highPointMarker.GetComponent<Renderer>();

        // Create a new material using Standard Shader
        Material mat = new Material(Shader.Find("Standard"));

        // Force the material into transparent mode
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        // Set Color with low alpha
        mat.color = new Color(1f, 0f, 0f, 0.15f);

        r.material = mat;

        highPointMarker.SetActive(false);
    }
}