using UnityEngine;
using TMPro;

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
        // 1. Switch the boolean flag
        isOnPageTwo = !isOnPageTwo;

        // 2. Toggle visibility (This triggers OnEnable in CardAnimator!)
        page1.SetActive(!isOnPageTwo);
        page2.SetActive(isOnPageTwo);

        // 3. Update Button Text
        if (buttonText != null)
        {
            buttonText.text = isOnPageTwo ? "Previous Page" : "Next Page";
        }
    }
}