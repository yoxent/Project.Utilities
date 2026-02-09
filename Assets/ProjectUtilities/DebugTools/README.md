## Debug & Diagnostics

### Overview
The DebugTools feature provides optional in-game diagnostics such as an FPS/memory overlay and a lightweight developer console for executing debug commands.

### Folder Structure
- **Runtime**: `DebugOverlay.cs`, `DevConsole.cs` and supporting types.
- **Editor**: Optional utilities for configuring debug commands and overlays.
- **Scene**: `Debug_Demo.unity` (optional) showing the overlay and console in action.

### Quick Setup
1. Copy `Assets/ProjectUtilities/DebugTools/` into your project.
2. Add a `DebugOverlay` prefab or component to your UI hierarchy and configure when it should be visible.
3. Optionally add `DevConsole` to a persistent object, register a few commands, and toggle it via input for development builds.

