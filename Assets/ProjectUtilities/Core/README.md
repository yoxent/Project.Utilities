## Core

### Overview
Core contains shared, low-level utilities that other ProjectUtilities features can depend on, such as the `ServiceLocator` and ScriptableObject-based event channels.

### Folder Structure
- **Runtime**: Core logic and ScriptableObjects.
  - `ServiceLocator.cs`
  - `EventChannels/` – event channel ScriptableObjects for decoupled communication.
- **Editor**: (Optional) future editor tooling related to core systems.

### Quick Setup

1. Copy `Assets/ProjectUtilities/Core/` into your project.
2. Add `ServiceLocator` to a bootstrap scene, or let it auto-create itself at first access.
3. Create `EventChannel` assets via the Create menu and reference them from systems that need to communicate without direct references.

### Public API Cheat Sheet

- **ServiceLocator**
  - `ServiceLocator.Instance.Register<T>(T service)` – register a service instance.
  - `T ServiceLocator.Instance.Get<T>()` – retrieve a service or `null` if none registered.
  - `ServiceLocator.Instance.Unregister<T>()` – remove a service.
  - **Used by:** Localization (LocalizationManager), Options (OptionsManager), Pooling (PoolManager). Register your service in a bootstrap or manager `Awake` before any `Get<T>()` from other systems.

- **EventChannel (no payload)**
  - `void Raise()` – invoke all listeners.
  - `void Subscribe(Action handler)` / `void Unsubscribe(Action handler)` – manage listeners.

### Integration Notes

- Keep Core free of references to gameplay- or UI-specific code.
- Use event channels instead of direct references when multiple systems need to react to the same event.

