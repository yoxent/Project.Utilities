using UnityEngine;

namespace ProjectUtilities.CanvasScaling.Core
{
    /// <summary>
    /// Data-driven scaling rules per device category. Assign to CanvasScalingManager.
    /// </summary>
    [CreateAssetMenu(fileName = "CanvasScalingProfile", menuName = "ProjectUtilities/CanvasScaling/Profile")]
    public class CanvasScalingProfile : ScriptableObject
    {
        [SerializeField] private CanvasScalingRule _defaultRule = new CanvasScalingRule();
        [SerializeField] private CanvasScalingRule _phoneRule = new CanvasScalingRule();
        [SerializeField] private CanvasScalingRule _tabletRule = new CanvasScalingRule();
        [SerializeField] private CanvasScalingRule _desktopRule = new CanvasScalingRule();

        /// <summary>
        /// Gets the rule for the given category. Falls back to default if not configured.
        /// </summary>
        public CanvasScalingRule GetRule(DeviceCategory category)
        {
            return category switch
            {
                DeviceCategory.Phone => _phoneRule,
                DeviceCategory.Tablet => _tabletRule,
                DeviceCategory.Desktop => _desktopRule,
                _ => _defaultRule
            };
        }
    }
}
