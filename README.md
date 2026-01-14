# AR-Squat-Lab 🏋️‍♂️📱

A Unity AR application that simulates biomechanical squat jumps using a physics-based ball. This project visualizes the conversion of Kinetic Energy into Potential Energy in real-time using Augmented Reality, now featuring an interactive gamified quiz system.

![Unity](https://img.shields.io/badge/Unity-6000.0.23f1-black?style=flat&logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android%20(ARCore)-green)

## 📖 Overview

This app transforms a standard smartphone into a physics laboratory. It detects real-world floor planes and spawns a 70 kg "virtual athlete" (represented by a sphere). The user interacts with the system to simulate a squat (eccentric phase) and a jump (concentric phase), while the app calculates and displays live physics data.

## 🎥 Demo Preview

<div align="center">
  <a href="https://github.com/user-attachments/assets/c56eb89e-fdf4-4834-9c67-f334f5fb2afd">
    <video src="https://github.com/user-attachments/assets/c56eb89e-fdf4-4834-9c67-f334f5fb2afd" width="400" />
  </a>
</div>

### Key Features
* **Dual Spawning Systems:**
    * **Scan Mode:** Detects a specific target image (e.g., laptop screen) to spawn the ball floating at eye level.
    * **Touch Mode:** Detects horizontal floor planes for "tap-to-spawn" functionality.
* **Interactive Physics:**
    * **Charge-to-Jump:** "Hold" button to squash the ball (load legs), "Release" to launch.
    * **Real-Time Data:** Displays Height ($h$), Potential Energy ($PE$), and Kinetic Energy ($KE$) live.
    * **Max Height Marker:** A visual Marker (Pink Circle) tracks the peak of your jump, resetting automatically for every new attempt.
* **Gamified Learning (Quiz Mode):**
    * A hybrid quiz system that tests both theory and practice.
    * **Concept Questions:** Multiple-choice theory (e.g., "What happens to KE at max height?").
    * **Physical Challenges:** AR-based tasks (e.g., "Perform a jump between 0.4m and 0.6m"). The app reads the ball's physics to grade your jump automatically!

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
* If using Scan Mode, the ball starts in **Floating Mode** (Gravity OFF).
* **Drag** the ball with your finger to position it precisely in your room.
* **Release** your finger to Drop the ball. It will fall to the floor and calibrate the height to 0.00m.

### 3. Squat & Jump
* **Squat:** Press and **HOLD** the "Squat" button. The ball will "squash" down to build charge.
* **Jump:** Release the button to launch the ball upward.
* **Analyze:**
    * **Live Data:** By default, see real-time Height, Potential Energy ($PE$), and Kinetic Energy ($KE$).
    * **Conservation Mode:** Tap the **"Energy / Data"** button to switch views. This compares your **Initial Kinetic Energy ($E_{k0}$)** at launch against your     **Final Potential Energy ($E_{pf}$)** at the peak to prove energy conservation.
    * **Marker:** A pink disk automatically marks the max height of every jump.

### 4. Take the Quiz
* Tap the **"Take Quiz"** button in either mode.
* Answer the theory questions.
* When a **Physical Challenge** appears (e.g., "Jump High"), use the Squat button to perform the required action with the ball. The app will automatically detect if you passed!
