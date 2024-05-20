namespace WATP.ECS
{
    public interface IStateComponent : IComponent, IEventComponent
    {
        public StateComponent StateComponent { get; }
    }

    /// <summary>
    /// entity중 상태를 처리하기 위한 component
    /// </summary>
    public class StateComponent : IComponent
    {
        public string State { get; set; } = "default";
    }
}