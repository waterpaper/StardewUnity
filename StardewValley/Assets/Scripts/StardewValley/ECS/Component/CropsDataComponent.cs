using Unity.Entities;

namespace WATP.ECS
{
    /// data component
    public struct CropsDataComponent : IComponentData
    {
        public int day;
        public int id;
    }

}