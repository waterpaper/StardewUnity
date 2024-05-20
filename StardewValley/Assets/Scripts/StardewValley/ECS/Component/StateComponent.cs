namespace WATP.ECS
{
    public interface IStateComponent : IComponent, IEventComponent
    {
        public StateComponent StateComponent { get; }
    }

    /// <summary>
    /// entity�� ���¸� ó���ϱ� ���� component
    /// </summary>
    public class StateComponent : IComponent
    {
        public string State { get; set; } = "default";
    }
}