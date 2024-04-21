using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// player 를 나타내는 component
    /// </summary>
    public struct PlayerComponent : IComponentData
    {
        public bool value;
    }
}