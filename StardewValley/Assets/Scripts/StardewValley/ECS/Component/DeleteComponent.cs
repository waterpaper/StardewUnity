using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// map object 중 system에서 삭제 처리가 필요한 entity가 가진 component
    /// </summary>
    public struct DeleteComponent : IComponentData
    {
        public bool isEnable;
        public bool isDelate;
        public bool isTimer;
        public float timer;
        public float deleteTime;
    }

}