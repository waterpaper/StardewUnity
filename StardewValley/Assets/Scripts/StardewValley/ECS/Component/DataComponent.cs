namespace WATP.ECS
{
    public interface IDataComponent : IComponent
    {
        public DataComponent DataComponent { get; }
    }

    /// <summary>
    /// �⺻ ������ ������ ������ �ִ� component
    /// </summary>
    [System.Serializable]
    public class DataComponent
    {
        public int datauid;
        public int id;
    }

}