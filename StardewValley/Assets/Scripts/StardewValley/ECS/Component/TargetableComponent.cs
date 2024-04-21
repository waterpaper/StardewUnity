using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// targeting이 가능한 component
    /// </summary>
    public struct TargetableComponent : IComponentData
    {
        public bool isEnable;
    }
}