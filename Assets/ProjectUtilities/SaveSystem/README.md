## Save & Persistence

### Overview
The SaveSystem feature provides an offline-first, versioned save framework with pluggable storage (local filesystem and optional cloud), serialization, migration, and conflict resolution.

### Folder Structure
- **Runtime/Core**: `SaveManager.cs`, `SaveGameData.cs`, `SaveSlotInfo.cs`, `SaveMetadata.cs`, `SaveSystemConfig.cs`, `SaveEvents.cs`.
- **Runtime/Storage**: `LocalFileStorage.cs` and optional cloud storage base classes.
- **Runtime/Serialization**: `JsonSaveSerializer.cs` and related helpers.
- **Runtime/Versioning**: `SaveVersion.cs`, `ISaveMigrationStep.cs`, `SaveMigrationPipeline.cs`.
- **Runtime/ConflictResolution**: `SaveConflictContext.cs`, `ISaveConflictResolver.cs`, `NewestWinsResolver.cs`.
- **Editor**: Inspectors and test tools for configs and migrations.
- **Scene**: `SaveSystem_Demo.unity` showing save/load flows and slot UI.

### Quick Setup
1. Copy `Assets/ProjectUtilities/SaveSystem/` into your project.
2. Create a `SaveSystemConfig` asset and configure local storage, serializer, and migration steps.
3. Implement your game-specific `MyGameSaveData : SaveGameData`.
4. Initialize `SaveManager` in a bootstrap scene and call its save/load APIs from your gameplay code or UI.

