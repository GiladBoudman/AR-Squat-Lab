# AR-Squat-Lab 🏋️‍♂️📱

A Unity AR application that simulates biomechanical squat jumps using a physics-based ball. This project visualizes the conversion of Kinetic Energy into Potential Energy in real-time using Augmented Reality.

![Unity](https://img.shields.io/badge/Unity-6000.0.23f1-black?style=flat&logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android%20(ARCore)-green)

## 📖 Overview

This app transforms a standard smartphone into a physics laboratory. It detects real-world floor planes and spawns a 70 - 90 kg "virtual athlete" (represented by a sphere). The user interacts with the system to simulate a squat (eccentric phase) and a jump (concentric phase), while the app calculates and displays live physics data.

### Key Features
* **Dual Spawning Systems:**
    * **Scan Mode:** Detects a specific target image (e.g., laptop screen) to spawn the ball floating at eye level.
    * **Touch Mode:** Detects horizontal floor planes for "tap-to-spawn" functionality.
* **Interactive Setup:**
    * **Drag-to-Move:** Users can grab the floating ball to reposition it precisely.
    * **Auto-Drop:** Releasing the ball enables gravity, causing it to fall and auto-calibrate the floor height ($h=0$).
* **Charge-to-Jump Mechanic:** "Hold" button to squash the ball (simulate loading legs), "Release" to launch.
* **Real-Time Physics Display:**
    * **Height ($h$):** Vertical displacement from the calibrated floor.
    * **Potential Energy ($PE$):** Calculated as $m \cdot g \cdot h$.
    * **Kinetic Energy ($KE$):** Calculated as $\frac{1}{2} m \cdot v^2$.


## ⚠️ Important for New Users

**Why is the scene empty?**
When you first open this project, the Hierarchy might look empty. This is normal! The `.gitignore` file excludes personal user settings, so Unity doesn't know which scene to open by default.

**How to fix it:**
1.  Go to the **Project Window** in Unity.
2.  Navigate to the **Assets** folder (or `Assets/Scenes`).
3.  Double-click the main scene file (e.g., `MainScene.unity`).
4.  The hierarchy will populate with the AR setup and scripts.


## 🎮 How to Use
### 1. Spawning the Athlete (ball)
You can choose between two modes from the Main Menu:
* **Scan Mode:** Point your camera at the designated target image. The ball will appear floating in front of it.
* **Touch Mode:** Point your camera at the floor until a grid appears, then tap to spawn.

### 2. Positioning (Setup Phase)
* The ball starts in **Floating Mode** (Gravity OFF).
* **Drag** the ball with your finger to position it perfectly in your room.
* **Release** your finger to Drop the ball. It will fall to the floor and set the height to 0.00m.

### 3. Squat & Jump
* **Squat:** Press and **HOLD** the "Squat" button. The ball will "squash" down to build charge.
* **Jump:** Release the button to launch the ball upward.
* **Analyze:** Watch the top-left UI to see the energy values change as the ball travels up and down.
