namespace WATP.ECS
{
    public interface IDelayDeleteComponent : IComponent, IEntity
    {
        public DelayDeleteComponent DelayDeleteComponent { get; }
    }

    [System.Serializable]
    public class DelayDeleteComponent
    {
        public bool isEnable = false;
        public float timer;
        public float deleteTime = 1;
    }

}