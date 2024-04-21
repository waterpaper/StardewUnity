using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// 충돌 처리가 필요한 entity가 가진 compoenent
    /// </summary>
    public struct ColliderComponent : IComponentData
    {
        public bool isEnable;
        public bool isCheck;
        public int colliderType;
        public float areaWidth;
        public float areaHeight;
    }
}