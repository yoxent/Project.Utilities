## Pooling

### Overview
The Pooling feature provides a generic, type-safe object pooling system with ScriptableObject-driven pool configurations and optional Canvas-based debug displays.

### Folder Structure
- **Runtime**: `Core/ObjectPool.cs`, `PoolManager.cs`, `IPoolable.cs`, `PoolConfig.cs`, `PoolGroupConfig.cs`; `UIAdapters/Canvas/PoolStatsDisplayUGUI.cs`.
- **Editor**: Custom inspectors for pool configs and optional analysis tools.
- **Scene**: `Pooling_Demo.unity` showing how to spawn and despawn pooled objects.

### Quick Setup
1. Copy `Assets/ProjectUtilities/Pooling/` into your project.
2. Create one or more `PoolConfig` assets for your prefabs, and optionally group them in `PoolGroupConfig` assets.
3. Initialize `PoolManager` (for example, in a bootstrap object) with the group configs.
4. Use `PoolManager.Rent<T>(poolId)` and `PoolManager.Return(poolId, instance)` to spawn and recycle objects.

