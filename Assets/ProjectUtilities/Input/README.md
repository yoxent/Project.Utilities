## Input Abstraction (New Input System)

### Overview
The Input feature provides a thin wrapper over Unity's New Input System to normalize input events across devices (touch, mouse/keyboard, gamepad) and expose them via events and configuration profiles.

### Folder Structure
- **Runtime**: `InputRouter.cs` and related helper types (e.g., action maps, sensitivity settings).
- **Editor**: Optional tools for visualizing and testing input bindings.
- **Scene**: `Input_Demo.unity` (optional) to demonstrate basic navigation and actions across devices.

### Quick Setup
1. Copy `Assets/ProjectUtilities/Input/` into your project.
2. Reference your `InputActionAsset` from `InputRouter`.
3. Subscribe to the router's events (e.g., move, look, confirm, cancel) in your gameplay and UI code.

