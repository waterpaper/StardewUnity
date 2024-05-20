namespace WATP.ECS
{
    public interface IMapObjectDataComponent : IComponent, IColliderComponent, ITransformComponent, IDelayDeleteComponent, IEventComponent
    {
        public MapObjectDataComponent MapObjectDataComponent { get; }
    }

    /// <summary>
    /// map object�� �⺻ ������ ���� component
    /// </summary>
    [System.Serializable]
    public class MapObjectDataComponent
    {
        public int id;
        public int hp;
    }

}