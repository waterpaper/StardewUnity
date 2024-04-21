using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// �浹 ó���� �ʿ��� entity�� ���� compoenent
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