## Localization

### Overview
The Localization feature provides a reusable, data-driven localization system using ScriptableObjects (and optionally JSON/CSV providers) plus **one component per text element** so it scales to any number of keys.

### Folder Structure
- **Runtime**: `Core/` (LocalizationManager, LocalizationConfig, LocalizedTextTable); **LocalizedTextTMP** (one key per TMP label); LocalizationDemoBootstrap.
- **Editor**: Table editor window, validators, and import/export helpers.
- **Scene**: `Localization_Demo.unity` showing LocalizedTextTMP on each label. Language switching is done from options via events.

### Scalable pattern: one component per text
- Use **LocalizedTextTMP** on each GameObject that has a `TextMeshProUGUI`: set **Localization Key** (e.g. `key_hello`, `menu_play`). No central list of keys — with 1000 labels you add 1000 components, each with one key. Text updates automatically when the language changes.
- In your game, use LocalizedTextTMP everywhere and trigger language changes from the options/settings UI via events (e.g. call `LocalizationManager.SetLanguage` when the user picks a language).

### Demo scene (Localization_Demo.unity)
- **LocalizationDemoBootstrap** initializes `LocalizationManager` with the **Configs** list (add your LocalizationConfig, e.g. `LocalizationConfig_EN`, as the first entry).
- **Title** and **Subtitle** each have **LocalizedTextTMP** with keys `key_hello` and `demo_subtitle`. Enter Play to see localized strings. To switch languages, call `LocalizationManager.SetLanguage` from your options UI (add more tables to the config for other languages).
- **Keyboard testing**: Add **LocalizationKeyboardSwitch** to a GameObject to switch between EN (key **1**) and ES (key **2**) at runtime. Configurable in the Inspector.

### Quick Setup
1. Copy `Assets/ProjectUtilities/Localization/` into your project.
2. Create a **LocalizationConfig** asset and one or more **LocalizedTextTable** assets (one table per language).
3. Initialize **LocalizationManager** early (e.g. via **LocalizationDemoBootstrap** in the demo scene or your own bootstrap).
4. Add **LocalizedTextTMP** to each TMP label and set its **Localization Key**. No central controller needed for text binding.

### Example: English and Spanish (one config, two tables)
You do **not** need multiple configs for multiple languages. Use **one** config and **one table per language**:

1. **Create two tables**
   - Right-click → Create → ProjectUtilities → Localization → TextTable.
   - **Table 1**: name e.g. `LocalizedTextTable_English`. Set **Language** to **English**. Add entries (e.g. `key_hello` → "Hello", `menu_play` → "Play").
   - **Table 2**: name e.g. `LocalizedTextTable_Spanish`. Set **Language** to **Spanish**. Add the same keys with Spanish values (`key_hello` → "Hola", `menu_play` → "Jugar").

2. **Create one config**
   - Right-click → Create → ProjectUtilities → Localization → Config.
   - Set **Default Language** to English (fallback when a key or locale is missing).
   - In **Tables**, add both tables (English and Spanish).

3. **Bootstrap**
   - Assign that config to **LocalizationDemoBootstrap** → **Configs** (add it as the first list entry).
   - At runtime, call `LocalizationManager.SetLanguage(LanguageCode.Spanish)` from your options UI to switch; all **LocalizedTextTMP** labels refresh automatically.

### Data model
- **Languages** are identified by the **LanguageCode** enum (e.g. English, Spanish).
- **String tables** are one **LocalizedTextTable** ScriptableObject per language; each table holds key/value entries.
- **LocalizationConfig** holds the default language and the list of tables; the manager loads all tables at init (no per-Get allocations).

### Multiple configs (optional)
- **Multiple configs** are for (1) merging separate *sets* of tables (e.g. base + DLC), or (2) **different tables per scene/context** (e.g. main menu vs gameplay). For “I just have English and Spanish everywhere,” use **one config** with **multiple tables** (one per language).
- **Bootstrap**: Assign one or more configs to **Configs**. One entry = single config; multiple entries = merged in order.

### Per-scene or per-context tables (multiple configs)
When different scenes use **different** English (and other language) tables — e.g. main menu has its own EN/ES tables, gameplay has separate EN/ES tables — use **one config per context** and **re-initialize** the manager when entering that scene:

1. **Config per context**
   - **Config_MainMenu**: Tables = [LocalizedTextTable_EN_Menu, LocalizedTextTable_ES_Menu], Default Language = English.
   - **Config_Gameplay**: Tables = [LocalizedTextTable_EN_Gameplay, LocalizedTextTable_ES_Gameplay], Default Language = English.

2. **Re-initialize on scene load**
   - When the main menu scene loads (or first scene with menu): get the manager (e.g. from ServiceLocator), call `manager.Initialize(Config_MainMenu)`.
   - When the gameplay scene loads: call `manager.Initialize(Config_Gameplay)`.
   - Same keys can exist in both (e.g. `title`) with different values per context; no need for key prefixes.

3. **How to wire it**
   - Put a bootstrap in each scene that assigns that scene’s config and calls `manager.Initialize(config)`, or use a central scene loader that calls `manager.Initialize(configForNextScene)` before/during load. The manager stays registered in ServiceLocator; re-initializing just replaces the loaded tables and refreshes the current language.

**Merging configs instead:** If you want both menu and gameplay keys available at once (e.g. overlay UI), use **Configs** with multiple configs: `Initialize([Config_MainMenu, Config_Gameplay])`. Later config overrides earlier for the same key, so use key prefixes (e.g. `menu_title`, `gameplay_title`) if both define the same key.

### Key convention
Use lowercase with underscores: **category_name**. Examples: `menu_play`, `options_volume`, `key_hello`, `demo_subtitle`. Missing keys display the key itself as fallback.

### How to add a new language
1. Create a new **LocalizedTextTable** asset (right-click → Create → ProjectUtilities → Localization → TextTable).
2. Set its **Language** to the new language (e.g. Spanish).
3. Add the table to your **LocalizationConfig** (Tables list).
4. Add entries (key/value) via the **Localization Table Editor** (Tools → ProjectUtilities → Localization Table Editor) or the table’s Inspector.

### Editor
- **Localization Table Editor** (Tools → ProjectUtilities → Localization Table Editor): assign a table, then **list**, **add**, **edit**, and **remove** entries. Changes apply to the selected table asset.

### Test scene
- **Localization_Demo.unity**: two labels with different keys; switch language from your options (via events) to verify both labels update.

### Future improvements
- CSV export/import for translation workflows is not in scope; consider adding later for external translators.

