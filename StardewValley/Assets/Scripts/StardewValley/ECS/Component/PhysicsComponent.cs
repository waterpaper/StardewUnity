using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// ���� component
    /// </summary>
    public struct PhysicsComponent : IComponentData
    {
        /// <summary> ������ ���ɿ��� </summary>
        public bool isEnable;

        /// <summary> ���� �̵� �ӵ� /// </summary>
        public float3 velocity;

        /// <summary> ���� /// </summary>
        public float weight;
    }
}