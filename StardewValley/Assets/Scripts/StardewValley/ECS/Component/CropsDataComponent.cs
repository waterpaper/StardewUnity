namespace WATP.ECS
{
    public interface ICropsDataComponent : IComponent, ITransformComponent, IEventComponent
    {
        public CropsDataComponent CropsDataComponent { get; }
    }

    /// <summary>
    /// 농작물 정보를 가지고 있는 component
    /// </summary>
    [System.Serializable]
    public class CropsDataComponent
    {
        public int day;
        public int id;
    }

}