# AR-Squat-Lab 🏋️‍♂️📱

A Unity AR application that simulates biomechanical squat jumps using a physics-based ball. This project visualizes the conversion of Kinetic Energy into Potential Energy in real-time using Augmented Reality.

![Unity](https://img.shields.io/badge/Unity-6000.0.23f1-black?style=flat&logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android%20(ARCore)-green)

## 📖 Overview

This app transforms a standard smartphone into a physics laboratory. It detects real-world floor planes and spawns a 70 - 90 kg "virtual athlete" (represented by a sphere). The user interacts with the system to simulate a squat (eccentric phase) and a jump (concentric phase), while the app calculates and displays live physics data.

### Key Features
* **AR Floor Detection:** Uses AR Foundation to detect and visualize horizontal planes.
* **Charge-to-Jump Mechanic:** "Hold" button to squash the ball (simulate loading legs), "Release" to launch.
* **Real-Time Physics:** displaying:
    * **Height ($h$):** Vertical displacement.
    * **Potential Energy ($PE$):** Calculated as $m \cdot g \cdot h$.
    * **Kinetic Energy ($KE$):** Calculated as $\frac{1}{2} m \cdot v^2$.
* **Safety Mechanics:** Auto-respawn system if the object clips through the AR plane (anti-tunneling).


## 🎮 How to Use
1.  **Scan:** Point your camera at the floor and move slowly until a grid appears.
2.  **Spawn:** Tap the grid to place the "Athlete Ball".
3.  **Squat:** Press and **HOLD** the "HOLD TO SQUAT" button. Watch the ball squash down.
4.  **Jump:** Release the button to launch.
5.  **Analyze:** Watch the UI text to see the Energy transfer in real-time.

```csharp
// Example Calculation Logic
float potentialEnergy = mass * gravity * currentHeight;
float kineticEnergy = 0.5f * mass * (velocity * velocity);
