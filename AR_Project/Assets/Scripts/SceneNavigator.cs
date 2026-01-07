using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This component handles navigation between different scenes in the application.
/// </summary>
public class SceneNavigator : MonoBehaviour
{
    public void LoadARScene()
    {
        SceneManager.LoadScene("SquatAR70");
    }

    public void LoadARSCANScene()
    {
        SceneManager.LoadScene("SquatAR70 (Scan)");
    }

    public void LoadPhysicsScene()
    {
        SceneManager.LoadScene("PhysicsInfo");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}