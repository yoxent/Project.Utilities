namespace ProjectUtilities.Pooling.Core
{
    /// <summary>
    /// Optional lifecycle callbacks for pooled objects.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}

