using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// 워프 가능 여부를 가진 component
    /// </summary>
    public struct WarpComponent : IComponentData
    {
         public bool isEnable;
    }
}