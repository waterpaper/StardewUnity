using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// 상태를 가진 component
    /// fsm에서 활용한다.
    /// </summary>
    public class StateComponent : IComponentData
    {
        public string State { get; set; } = "default";
    }
}