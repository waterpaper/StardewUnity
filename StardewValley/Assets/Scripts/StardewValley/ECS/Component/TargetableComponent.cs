using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// targeting�� ������ component
    /// </summary>
    public struct TargetableComponent : IComponentData
    {
        public bool isEnable;
    }
}