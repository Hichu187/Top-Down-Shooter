# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A top-down shooter game built on **Unity 6 (6000.3.6f1)** with Universal Render Pipeline (URP). The project targets both Android mobile (with virtual joystick input) and desktop platforms.

## Development Workflow

**Running the project:** Open in Unity Hub with Unity 6000.3.6f1, load `Assets/Scenes/SampleScene.unity`, press Ctrl+P to play.

**Building:** File > Build Settings. Primary target is Android 13 (landscape orientation, 1024x768). Desktop (Standalone) is also supported.

**Editing code:** VS Code (configured in `.vscode/`) or Visual Studio (`.vsconfig` requires ManagedGame workload) or Rider — all are configured. Attach debugger via "Attach to Unity" (vstuc).

**Testing:** Unity Test Framework (`com.unity.test-framework`) is included. Tests run via Unity Editor: Window > General > Test Runner.

## Architecture

### Input

Two parallel input systems coexist:

- **New Input System** (`Assets/InputSystem_Actions.inputactions`) — defines actions: Move, Look, Attack, Interact, Crouch, Jump, Sprint, Previous, Next. Gamepad and keyboard bindings configured.
- **Joystick Pack** (`Assets/Joystick Pack/`) — touch-friendly virtual joystick UI using Unity EventSystem interfaces. Four joystick types: Fixed, Dynamic, Floating, Variable. Reference `JoystickPlayerExample.cs` for the integration pattern (read `joystick.Direction` in `FixedUpdate`, apply `Rigidbody.AddForce`).

### Player

`Assets/Scripts/Player/PlayerMovement.cs` is the main entry point for gameplay logic — currently a stub. Follow the `JoystickPlayerExample.cs` pattern for physics-based movement: collect input in `Update`, apply forces in `FixedUpdate`.

### Rendering

URP with separate quality assets: `Settings/PC_RPAsset.asset` and `Settings/Mobile_RPAsset.asset`. Post-processing via Volume profiles (`DefaultVolumeProfile.asset`, `SampleSceneProfile.asset`).

### Key Packages

| Package | Purpose |
|---|---|
| `com.baxoai.framework` | Custom game framework (from GitHub) |
| `com.cysharp.unitask` | Async/await (prefer over coroutines) |
| `com.unity.addressables` | Asset loading (remote/local) |
| `com.unity.ai.navigation` | NavMesh for enemy AI |
| `com.unity.inputsystem` | New Input System |
| `com.unity.timeline` | Cutscene/animation sequences |

### Scene Structure

Single scene: `Assets/Scenes/SampleScene.unity`. Main Camera has AudioListener enabled. All gameplay objects should be placed here or instantiated at runtime.
