namespace WATP.ECS
{
    public interface IPlayerComponent : IComponent, ITransformComponent, IEventComponent
    {
        public PlayerComponent PlayerComponent { get; }
    }

    /// <summary>
    /// entity중 player를 나타내는 component
    /// </summary>
    [System.Serializable]
    public class PlayerComponent
    {
        public bool value;
    }
}