using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// ���¸� ���� component
    /// fsm���� Ȱ���Ѵ�.
    /// </summary>
    public class StateComponent : IComponentData
    {
        public string State { get; set; } = "default";
    }
}