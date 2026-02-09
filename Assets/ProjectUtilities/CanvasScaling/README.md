## Canvas Scaling Manager

### Overview
The CanvasScaling feature manages CanvasScaler settings across different devices and window sizes, using data-driven rules to handle mobile aspect ratios, orientation changes, and desktop window resizing.

### Folder Structure
- **Runtime**: `Core/CanvasScalingManager.cs`, `CanvasScalingProfile.cs`, `CanvasScalingRule.cs`, `DeviceCategory.cs`; `Adapters/CanvasScalerAdapter.cs`.
- **Editor**: `CanvasScalingProfileEditor.cs` for managing scaling rules in the inspector.
- **Scene**: `CanvasScaling_Demo.unity` demonstrating how different aspect ratios activate different scaling rules.

### Quick Setup
1. Copy `Assets/ProjectUtilities/CanvasScaling/` into your project.
2. Create a `CanvasScalingProfile` asset (right‑click in Project → Create → ProjectUtilities → CanvasScaling → Profile) and set reference resolutions and match values per device category (Default, Phone, Tablet, Desktop).
3. Add a `CanvasScalingManager` to a bootstrap or persistent GameObject, assign the profile, and ensure every Canvas that should follow the profile has a `CanvasScalerAdapter` on the same GameObject as the Canvas/CanvasScaler.

### Demo scene (CanvasScaling_Demo.unity)
- The scene contains **CanvasScalingManager** (empty GameObject) and **DemoCanvas** (Canvas + CanvasScaler).
- **After Unity compiles:** Add the **CanvasScalingManager** component to the CanvasScalingManager GameObject and the **CanvasScalerAdapter** component to DemoCanvas. Create a profile (see Quick Setup) and assign it to the manager’s Profile field.
- Resize the Game view to see scaling switch by aspect ratio (e.g. narrow = Phone, wide = Desktop).

