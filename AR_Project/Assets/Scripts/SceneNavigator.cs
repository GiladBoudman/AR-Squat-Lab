using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void LoadQuizScene()
    {
        SceneManager.LoadScene("QuizScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}