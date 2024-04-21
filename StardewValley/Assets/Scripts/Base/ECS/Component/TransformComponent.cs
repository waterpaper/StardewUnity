using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 월드 기본값을 가지고 있는 component
    /// </summary>
    public struct TransformComponent :IComponentData
    {
        public float3 position;
        public float3 rotation;
    }
}