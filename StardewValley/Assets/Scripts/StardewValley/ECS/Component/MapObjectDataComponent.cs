using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// entity�� ���� data component
    /// </summary>
    public struct MapObjectDataComponent : IComponentData
    {
        public int id;
        public int hp;
    }

}