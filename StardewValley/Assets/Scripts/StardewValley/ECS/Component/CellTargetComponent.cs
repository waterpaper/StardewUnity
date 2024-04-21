using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    public struct CellPath : IBufferElementData
    {
        public float2 value;
    }

    /// <summary>
    /// path finding (cell)이 필요한 entity가 가진 component
    /// </summary>
    public struct CellTargetComponent : IComponentData
    {
        /// <summary> 움직임 가능여부 </summary>
        public bool isEnable;
        /// <summary> 목표 /// </summary>
        public bool finish;
        /// <summary> 목표 /// </summary>
        public float refreshTime;
        /// <summary> 목표 /// </summary>
        public float refreshTimer;
        /// <summary> 이전 포지션 /// </summary>
        public DynamicBuffer<CellPath> cellPaths;
    }
}