using UnityEngine;
using TMPro;

/// <summary>
/// Handles pagination between two pages of physics information in the UI.
/// </summary>
public class PhysicsInfoPaging : MonoBehaviour
{
    [Header("Pages")]
    public GameObject page1;
    public GameObject page2;

    [Header("UI Elements")]
    public TextMeshProUGUI buttonText;

    private bool isOnPageTwo = false;

    public void TogglePages()
    {
        // Switch the boolean flag
        isOnPageTwo = !isOnPageTwo;

        // Toggle visibility (This triggers OnEnable in CardAnimator!)
        page1.SetActive(!isOnPageTwo);
        page2.SetActive(isOnPageTwo);

        // Update Button Text
        if (buttonText != null)
        {
            buttonText.text = isOnPageTwo ? "Previous Page" : "Next Page";
        }
    }
}