using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// player �� ��Ÿ���� component
    /// </summary>
    public struct PlayerComponent : IComponentData
    {
        public bool value;
    }
}