namespace WATP.ECS
{
    public interface IPlayerComponent : IComponent, ITransformComponent, IEventComponent
    {
        public PlayerComponent PlayerComponent { get; }
    }

    [System.Serializable]
    public class PlayerComponent
    {
        public bool value;
    }
}