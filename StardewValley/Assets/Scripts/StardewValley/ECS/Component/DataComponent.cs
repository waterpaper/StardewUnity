namespace WATP.ECS
{
    public interface IDataComponent : IComponent
    {
        public DataComponent DataComponent { get; }
    }

    [System.Serializable]
    public class DataComponent
    {
        public int datauid;
        public int id;
    }

}