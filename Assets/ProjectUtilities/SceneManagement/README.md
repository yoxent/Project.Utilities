## Scene Management & Bootstrapping

### Overview
The SceneManagement feature provides a simple `SceneLoader` wrapper for async scene loading (single and additive) plus a `Bootstrapper` that ensures core managers exist once and persist across scenes.

### Folder Structure
- **Runtime**: `SceneLoader.cs`, `Bootstrapper.cs`.
- **Editor**: Optional helpers for configuring build indices or addressable scene keys.
- **Scene**: `SceneLoader_Demo.unity` (optional) to demonstrate transitions and loading overlays.

### Quick Setup
1. Copy `Assets/ProjectUtilities/SceneManagement/` into your project.
2. Add a `Bootstrapper` to a boot scene and assign references to core managers if needed.
3. Use `SceneLoader` APIs to move between scenes or load additively, and hook progress events into your UI.

