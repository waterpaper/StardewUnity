using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// input 중 아이템 사용 입력시 처리되야 하는 entity가 가진 component
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