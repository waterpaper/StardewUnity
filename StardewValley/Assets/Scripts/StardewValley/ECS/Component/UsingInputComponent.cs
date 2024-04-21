using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// input �� ������ ��� �Է½� ó���Ǿ� �ϴ� entity�� ���� component
    /// </summary>
    public struct UsingInputComponent : IComponentData
    {
        public bool isEnable;
        public bool isAction;
        public float actionTimer;
        public int actionType;
        public float3 targetPos;
    }
}