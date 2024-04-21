using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    ///  entity°¡ °¡Áø data component
    /// </summary>
    public struct HoedirtDataComponent : IComponentData
    {
        public bool watering;
        public bool add;
        public bool up;
        public bool left;
        public bool down;
        public bool right;
    }

}