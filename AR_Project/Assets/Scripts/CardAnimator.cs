using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This component animates child card objects to pop up in a cascading sequence when the scene starts.
/// </summary>
public class CardAnimator : MonoBehaviour
{
    [Header("Settings")]
    public float animationSpeed = 0.1f; // How fast the cards pop up in sequence
    public float slideDuration = 0.3f; // How long each card takes to pop up

    void OnEnable() // Changed from Start() to OnEnable()
    {
        // Now this happens every time the page is set to Active
        StartCoroutine(AnimateCards());
    }

    IEnumerator AnimateCards()
    {
        // Get all the child cards attached to this object
        int childCount = transform.childCount;

        // Hide all cards initially
        for (int i = 0; i < childCount; i++)
        {
            Transform card = transform.GetChild(i);
            // Set scale to 0 (invisible)
            card.localScale = Vector3.zero;
        }

        // Animate each card in sequence
        for (int i = 0; i < childCount; i++)
        {
            Transform card = transform.GetChild(i);

            // Start the pop up motion for this specific card
            StartCoroutine(PopUpCard(card));

            // Making the cascade effect by waiting before starting the next one
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    IEnumerator PopUpCard(Transform card)
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / slideDuration;

            // Makes the movement start slow, go fast, end slow
            float scale = Mathf.SmoothStep(0, 1, timer);

            card.localScale = new Vector3(scale, scale, scale);

            yield return null; // Wait for next frame
        }

        // Ensure it ends perfectly at size 1
        card.localScale = Vector3.one;
    }
}