using Unity.Entities;

namespace WATP.ECS
{
    public struct DataComponent : IComponentData
    {
        public int dataUid;
        public int id;
    }
}