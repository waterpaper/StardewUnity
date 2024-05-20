namespace WATP.ECS
{
    public interface IPlayerComponent : IComponent, ITransformComponent, IEventComponent
    {
        public PlayerComponent PlayerComponent { get; }
    }

    /// <summary>
    /// entity�� player�� ��Ÿ���� component
    /// </summary>
    [System.Serializable]
    public class PlayerComponent
    {
        public bool value;
    }
}