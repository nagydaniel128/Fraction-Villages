# Fraction-Villages
## Description
This is a Unity-based 3D prototype focusing on AI-driven faction behavior and coordinated combat systems.
The project simulates a small village environment where multiple factions operate autonomously using state-machine-based decision logic.

The main goal of the project was to experiment with AI coordination, combat interaction and systemic gameplay behavior rather than visuals.

## Features
- Central village-level decision logic with individual AI state machines
- Autonomous task assignment (guarding, patrolling, resource gathering, outpost building)
- Health-based self-preservation logic (units retreat to heal when injured)
- Alert system: units notify nearby allies when detecting enemies
- Coordinated combat behavior:
 - Target information sharing between allies
 - Surround positioning logic
 - Stun, shield, shield-piercing and dodge mechanics
- Multiple factions with different base stats and hostile interactions
- Dynamic large-scale combat scenarios
- Day/Night cycle

## Installation and Running
1. **Clone the repository**:
    ```bash
    git clone https://github.com/nagydaniel128/Fraction-Villages.git
    ```

2. **Open the project in Unity**:
   - Launch the Unity Editor.
   - Open the 'Procedural map generation' folder located in the cloned project via `File > Open Project`.

## Developer Information
### System Requirements
- Unity version: 2021.3.22f1
- Platform: Windows

### Building the Game
If you want to build the game yourself:
1. Open the project in Unity.
2. Select `File > Build Settings`.
3. Set your desired platform and other settings.
4. Click the `Build` button.
