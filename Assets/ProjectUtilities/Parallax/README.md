## Parallax

### Overview
Parallax provides a reusable background parallax system for 2D and 3D menus and scenes. It lets you move multiple layers at different speeds based on camera movement, pointer position, or automatic scrolling.

### Folder Structure
- **Runtime/Core**: `ParallaxLayer.cs`, `ParallaxController.cs`, `ParallaxConfig.cs`.
- **Runtime/UIAdapters/Canvas**: `ParallaxMenuBackground.cs` for quickly wiring Canvas-based menu backgrounds.
- **Editor**: Optional editors and tooling for parallax configs.
- **Scene**: `Parallax_Demo.unity` with a sample layered background.

### Quick Setup
1. Copy `Assets/ProjectUtilities/Parallax/` into your project.
2. Add `ParallaxController` to an empty GameObject in your menu or gameplay scene and assign a `ParallaxConfig`.
3. Add `ParallaxLayer` to each background layer (e.g., RectTransforms or sprites) and assign them to the controller (or use `ParallaxMenuBackground`).
4. Press Play and adjust depth factors and intensity until the motion feels right.

