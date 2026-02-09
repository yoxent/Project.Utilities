## ProjectUtilities – Unity Utility Collection

ProjectUtilities is a **one-stop collection of reusable utilities** designed to help you build Unity projects faster and more consistently. It targets **Unity 6 (6000.x)**, uses the **new Input System**, and is structured for reuse across multiple games (menus, gameplay, UI, and tooling).

### Features at a glance

- **Core**
  - `ServiceLocator` for simple dependency lookup
  - ScriptableObject **Event Channels** for decoupled messaging
- **Pooling**
  - Generic `ObjectPool<T>` built on `UnityEngine.Pool.ObjectPool<T>`
  - `IPoolable` lifecycle (`OnSpawned` / `OnDespawned`)
- **Parallax**
  - `ParallaxLayer` with three modes:
    - **Pointer-based** (mouse-driven)
    - **Auto Scroll** (continuous 2D scrolling with seamless tiling)
    - **Camera-based** (camera-follow parallax with wrapping tiles)
  - Configurable via `ParallaxConfig` and demo scene
- **Options**
  - ScriptableObject-based options profile
  - UGUI bindings: sliders, toggles, dropdowns, and demo options menu
- **Number Formatting**
  - Configurable number formatting for large values (e.g. “1.2K”, “3.4M”)
- **SceneManagement**
  - Scene bootstrapper, loading screen controller, and demo scenes
- **DebugTools**
  - In‑game debug overlay and dev console
- **Localization / SaveSystem / Others**
  - Additional modules for common game infra (see individual READMEs / demos)

---

## Requirements

- **Unity**: 6000.3.2f1 (Unity 6.x)
- **Rendering**: URP
- **Input**: New Input System
- **Scripting Runtime**: .NET Standard 2.1

---

## Getting started

### Installation

1. **Clone or add as submodule**

```bash
git clone https://github.com/yoxent/Project.Utilities.git
```

2. Open your Unity project and add `Assets/ProjectUtilities` into your project (or mount as a nested project/package if you prefer).

3. Open any of the demo scenes to see the systems in action (e.g. `Parallax_Demo`, `OptionsMenu_Demo`, `Debug_Demo`, `SM_Menu`).

---

## Folder overview

Under `Assets/ProjectUtilities/`:

- `Core/`
  - `Runtime/ServiceLocator.cs`
  - `Runtime/EventChannels/…`
  - `Editor/Tests/ServiceLocatorEditModeTest.cs`
- `Pooling/`
  - `Runtime/Core/ObjectPool.cs`, `IPoolable.cs`, `PoolConfig.asset`
  - `Editor/Tests/ObjectPoolEditModeTest.cs`
- `Parallax/`
  - `Runtime/Core/ParallaxLayer.cs`, `ParallaxController.cs`, `ParallaxConfig.cs`, `CameraKeyboardMover.cs`
  - `Scene/Parallax_Demo.unity`
  - `README.md`
- `Options/`
  - Runtime components + `OptionsMenu_Demo.unity`
- `NumberFormatting/`
  - ScriptableObject config + `BigNumbers_Demo.unity`
- `SceneManagement/`
  - `Bootstrapper`, `SceneLoader`, loading screen context + multiple demo scenes
- `DebugTools/`
  - `DebugOverlay`, `DevConsole` + `Debug_Demo.unity`
- `PLAN_AND_TRACKING.md`
  - High-level roadmap and Jira mapping for this utilities pack

---

## Usage examples

### Parallax

1. **Setup**

- Add a `ParallaxController` to a GameObject (e.g. in your menu or level).
- Assign a `ParallaxConfig` (e.g. `ParallaxDemo_Config.asset`).
- Add one or more `ParallaxLayer` components to background GameObjects and reference them from the controller.

2. **Pointer-based mode**

- Set controller `Mode` to `PointerBased`.
- For each `ParallaxLayer`:
  - Set **Depth Factor** (far background ≈ 0.3–0.7, near foreground ≈ 1.0–1.5).
  - Adjust **Pointer Strength** for how much that layer should move.
- The controller reads the mouse via the new Input System and calls `ParallaxLayer.ApplyOffset` every frame.

3. **Auto Scroll mode**

- Set controller `Mode` to `AutoScroll`.
- On each `ParallaxLayer`:
  - Configure **Scroll Speed** (Vector2; sign controls direction).
  - Enable **Loop** and **Duplicate Tile For Seamless Scroll**.
  - Optionally assign a child `Tile` transform for clean layering.

4. **Camera-based mode**

- Set controller `Mode` to `CameraBased`.
- `ParallaxController` uses `Camera.main` and a stored reference position.
- Each `ParallaxLayer`:
  - Uses `Scroll Factor` (0 = fixed, 1 = follow camera exactly).
  - Optionally uses looping + duplicate tile to create continuous scrolling backgrounds when the camera moves.

### Pooling

1. **Implement `IPoolable`**

```csharp
public class Bullet : MonoBehaviour, IPoolable
{
    public void OnSpawned()
    {
        // Reset state, enable effects, etc.
    }

    public void OnDespawned()
    {
        // Stop effects, reset timers, etc.
    }
}
```

2. **Create a pool**

```csharp
var poolRoot = new GameObject("BulletPool").transform;
var bulletPrefab = /* reference to Bullet component on your prefab */;
var bulletPool = new ObjectPool<Bullet>(
    prefab: bulletPrefab,
    initialSize: 10,
    maxSize: 100,
    autoExpand: true,
    parent: poolRoot);
```

3. **Use the pool**

```csharp
// Rent
var bullet = bulletPool.Rent();
if (bullet != null)
{
    bullet.transform.position = spawnPosition;
    bullet.transform.rotation = spawnRotation;
}

// Return
bulletPool.Return(bullet);
```

---

## Tests

- **Core / ServiceLocator**
  - EditMode tests in `Core/Editor/Tests/ServiceLocatorEditModeTest.cs`
- **Pooling**
  - EditMode tests in `Pooling/Editor/Tests/ObjectPoolEditModeTest.cs`
- **Parallax**
  - EditMode tests in `Core/Editor/Tests/ParallaxEditModeTest.cs` cover:
    - Pointer-based clamping & pointer strength
    - Auto-scroll movement without duplicate
    - Camera-based parallax with and without looping

Run tests via **Unity Test Runner** (EditMode) after importing the package.

---

## Conventions

- C# 12 / Unity 6 conventions:
  - Private fields: `_camelCase`
  - Public properties: `PascalCase`
  - No public fields except for serialized Unity data where necessary.
- UI: UGUI / Canvas-based by default.
- Asset loading: prefer **Addressables** for new content (see project rules when extending).

---
