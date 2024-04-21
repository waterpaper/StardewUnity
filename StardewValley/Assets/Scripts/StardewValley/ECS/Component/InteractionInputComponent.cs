using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// input 중 상호작용 키 입력시 처리되야 하는 entity가 가진 component
    /// </summary>
    public struct InteractionInputComponent : IComponentData
    {
        /// <summary> 가능여부 </summary>
        public bool isEnable;
    }
}