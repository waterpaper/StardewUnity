using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    /// <summary>
    /// 움직이는 속성이 있는 entity가 가진 component
    /// </summary>
    public struct MoveComponent : IComponentData
    {
        /// <summary> 움직임 가능여부 </summary>
        public bool isEnable;
        /// <summary> 목표 /// </summary>
        public float3 targetPos;
        /// <summary> 이전 포지션 /// </summary>
        public float3 beforePos;
        /// <summary> 현재 이동 속도 /// </summary>
        public float speed;
        /// <summary> 이번 턴 움직임 유무 /// </summary>
        public bool isMoveTurn;
    }
}