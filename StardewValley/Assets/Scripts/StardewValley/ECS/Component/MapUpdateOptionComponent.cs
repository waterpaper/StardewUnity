
using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// map update component
    /// </summary>
    public struct MapUpdateOptionComponent : IComponentData
    {
        public bool isDay;
        public bool isHoedirt;
    }
}