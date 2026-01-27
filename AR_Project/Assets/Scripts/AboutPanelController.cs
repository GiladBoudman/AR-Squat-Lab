using UnityEngine;

/// <summary>
/// Controls the About Panel UI functionality.
/// </summary> <summary>
public class AboutPanelController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject AboutPanel; // Drag your Panel here

    void Start()
    {
        // Make sure the panel is hidden when the scene starts
        if (AboutPanel != null)
        {
            AboutPanel.SetActive(false);
        }
    }

    public void OpenAbout()
    {
        if (AboutPanel != null) AboutPanel.SetActive(true);
    }

    public void CloseAbout()
    {
        if (AboutPanel != null) AboutPanel.SetActive(false);
    }
}