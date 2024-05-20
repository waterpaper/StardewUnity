namespace WATP.ECS
{
    public interface IDataComponent : IComponent
    {
        public DataComponent DataComponent { get; }
    }

    /// <summary>
    /// 기본 데이터 정보를 가지고 있는 component
    /// </summary>
    [System.Serializable]
    public class DataComponent
    {
        public int datauid;
        public int id;
    }

}