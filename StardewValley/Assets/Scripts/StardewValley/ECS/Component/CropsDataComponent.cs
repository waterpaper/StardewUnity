namespace WATP.ECS
{
    public interface ICropsDataComponent : IComponent, ITransformComponent, IEventComponent
    {
        public CropsDataComponent CropsDataComponent { get; }
    }

    [System.Serializable]
    public class CropsDataComponent
    {
        public int day;
        public int id;
    }

}