using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// �����̴� �Ӽ��� �ִ� entity�� ���� component
    /// </summary>
    public struct MoveComponent : IComponentData
    {
        /// <summary> ������ ���ɿ��� </summary>
        public bool isEnable;
        /// <summary> ��ǥ /// </summary>
        public float3 targetPos;
        /// <summary> ���� ������ /// </summary>
        public float3 beforePos;
        /// <summary> ���� �̵� �ӵ� /// </summary>
        public float speed;
        /// <summary> �̹� �� ������ ���� /// </summary>
        public bool isMoveTurn;
    }
}