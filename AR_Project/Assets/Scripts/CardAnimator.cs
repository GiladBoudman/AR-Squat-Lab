using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Needed for UI elements

public class CardAnimator : MonoBehaviour
{
    [Header("Settings")]
    public float animationSpeed = 0.1f; // How fast they pop in sequence
    public float slideDuration = 0.3f; // How long the slide takes per card

    void Start()
    {
        // Automatically start the animation when the scene opens
        StartCoroutine(AnimateCards());
    }

    IEnumerator AnimateCards()
    {
        // Get all the child cards attached to this object
        // We skip "transform" (which is the parent itself) and grab only children
        int childCount = transform.childCount;

        // Hide them all initially
        for (int i = 0; i < childCount; i++)
        {
            Transform card = transform.GetChild(i);
            // Set scale to 0 (invisible)
            card.localScale = Vector3.zero;
        }

        // Animate them one by one
        for (int i = 0; i < childCount; i++)
        {
            Transform card = transform.GetChild(i);

            // Start the "Pop Up" motion for this specific card
            StartCoroutine(PopUpCard(card));

            // Wait a tiny bit before triggering the next one (The "Cascade" effect)
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    IEnumerator PopUpCard(Transform card)
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / slideDuration;

            // "SmoothStep" makes the movement start slow, go fast, end slow
            float scale = Mathf.SmoothStep(0, 1, timer);

            card.localScale = new Vector3(scale, scale, scale);

            yield return null; // Wait for next frame
        }

        // Ensure it ends perfectly at size 1
        card.localScale = Vector3.one;
    }
}