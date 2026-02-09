## Number Formatting & Abbreviations

### Overview
The NumberFormatting feature provides utilities for formatting large numbers for incremental/idle games, including configurable suffix tiers (K, M, B, T, etc.), decimal places, thousands separator, and optional prefix (e.g. $).

### Folder Structure
- **Runtime**: `NumberFormatter.cs`, `NumberFormattingConfig.cs` (ScriptableObject with suffix tiers and formatting options).
- **Editor**: `NumberFormattingConfigEditor.cs` — custom inspector with live preview.
- **Scene**: `BigNumbers_Demo.unity` (optional) to visualize formats in a simple HUD.

### Demo scene (BigNumbers_Demo.unity)
- **DemoCanvas** has **BigNumbersDemoController** with **MainLabel** and **SamplesLabel** (TMP). Assign a **NumberFormattingConfig** to the controller’s **Config** field and enter Play: the main value and sample lines update every frame.
- Create a config via **Create → ProjectUtilities → NumberFormatting → Config** and add suffix tiers (e.g. 1000 / K, 1e6 / M, 1e9 / B, 1e12 / T).

### Quick Setup
1. Copy `Assets/ProjectUtilities/NumberFormatting/` into your project.
2. Create a **NumberFormattingConfig** asset (right‑click → Create → ProjectUtilities → NumberFormatting → Config).
3. Add **Suffix Tiers** (e.g. threshold 1000 / suffix "K", 1000000 / "M", 1e9 / "B", 1e12 / "T"). Tiers are auto-sorted by threshold.
4. Use **NumberFormatter.Format(value, config)** or attach a **NumberFormatter** component and set **Config**, then call **Format(value)**.

### Usage
```csharp
// Static
string text = NumberFormatter.Format(1_234_567, config);  // e.g. "1.23M"

// Instance (e.g. on a MonoBehaviour with [SerializeField] NumberFormattingConfig _config)
var formatter = new NumberFormatter(_config);
label.text = formatter.Format(score);
```

