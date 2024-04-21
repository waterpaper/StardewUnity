using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 물리 component
    /// </summary>
    public struct PhysicsComponent : IComponentData
    {
        /// <summary> 움직임 가능여부 </summary>
        public bool isEnable;

        /// <summary> 현재 이동 속도 /// </summary>
        public float3 velocity;

        /// <summary> 무게 /// </summary>
        public float weight;
    }
}