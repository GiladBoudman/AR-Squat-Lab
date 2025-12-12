using UnityEngine;

public class UIBridge : MonoBehaviour
{
    private SquatPhysicsController ballPhysics;

    public void OnPress()
    {
        FindBall();
        if (ballPhysics != null) ballPhysics.StartSquat();
    }

    public void OnRelease()
    {
        if (ballPhysics != null) ballPhysics.ReleaseJump();
    }

    private void FindBall()
    {
        if (ballPhysics == null)
        {
            // This looks for the ball we spawned by its tag
            GameObject ball = GameObject.FindWithTag("Player");
            if (ball != null) ballPhysics = ball.GetComponent<SquatPhysicsController>();
        }
    }
}