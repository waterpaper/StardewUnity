namespace WATP.ECS
{
    public interface IStateComponent : IComponent, IEventComponent
    {
        public StateComponent StateComponent { get; }
    }

    public class StateComponent : IComponent
    {
        public string State { get; set; } = "default";
    }
}