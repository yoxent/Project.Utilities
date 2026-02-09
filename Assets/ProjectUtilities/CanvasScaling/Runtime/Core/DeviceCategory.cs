namespace ProjectUtilities.CanvasScaling.Core
{
    /// <summary>
    /// Broad device/screen category used to select a scaling rule.
    /// </summary>
    public enum DeviceCategory
    {
        /// <summary>Fallback when no specific category matches.</summary>
        Default = 0,

        /// <summary>Narrow, portrait-first (e.g. phone).</summary>
        Phone = 1,

        /// <summary>Medium aspect (e.g. tablet).</summary>
        Tablet = 2,

        /// <summary>Wide aspect (e.g. desktop, TV).</summary>
        Desktop = 3
    }
}
