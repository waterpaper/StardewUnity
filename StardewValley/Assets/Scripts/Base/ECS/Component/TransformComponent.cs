using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// ���� �⺻���� ������ �ִ� component
    /// </summary>
    public struct TransformComponent :IComponentData
    {
        public float3 position;
        public float3 rotation;
    }
}