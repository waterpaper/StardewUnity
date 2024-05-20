namespace WATP.ECS
{
    public interface ICropsDataComponent : IComponent, ITransformComponent, IEventComponent
    {
        public CropsDataComponent CropsDataComponent { get; }
    }

    /// <summary>
    /// ���۹� ������ ������ �ִ� component
    /// </summary>
    [System.Serializable]
    public class CropsDataComponent
    {
        public int day;
        public int id;
    }

}