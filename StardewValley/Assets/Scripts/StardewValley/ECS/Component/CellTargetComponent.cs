using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    public struct CellPath : IBufferElementData
    {
        public float2 value;
    }

    /// <summary>
    /// path finding (cell)�� �ʿ��� entity�� ���� component
    /// </summary>
    public struct CellTargetComponent : IComponentData
    {
        /// <summary> ������ ���ɿ��� </summary>
        public bool isEnable;
        /// <summary> ��ǥ /// </summary>
        public bool finish;
        /// <summary> ��ǥ /// </summary>
        public float refreshTime;
        /// <summary> ��ǥ /// </summary>
        public float refreshTimer;
        /// <summary> ���� ������ /// </summary>
        public DynamicBuffer<CellPath> cellPaths;
    }
}