using UnityEngine;

/// <summary>
/// This component bridges UI button events to the SquatPhysicsController on the ball
/// </summary>
public class UIBridge : MonoBehaviour
{
    private SquatPhysicsController ballPhysics;

    // Called when the user presses down on the UI button
    public void OnPress()
    {
        FindBall();
        if (ballPhysics != null) ballPhysics.StartSquat();
    }

    // Called when the user releases the UI button
    public void OnRelease()
    {
        if (ballPhysics != null) ballPhysics.ReleaseJump();
    }

    // Called when the user toggles the energy mode button
    public void OnToggleEnergy()
    {
        FindBall(); // Ensure we have found the ball
        
        if (ballPhysics != null) 
        {
            ballPhysics.ToggleEnergyMode(); // Call the toggle function
        }
    }

    // Finds the ball in the scene if we haven't already
    private void FindBall()
    {
        if (ballPhysics == null)
        {
            // Looks for the ball we spawned by its tag
            GameObject ball = GameObject.FindWithTag("Player");
            if (ball != null) ballPhysics = ball.GetComponent<SquatPhysicsController>();
        }
    }
}