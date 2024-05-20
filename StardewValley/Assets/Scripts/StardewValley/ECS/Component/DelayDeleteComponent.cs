namespace WATP.ECS
{
    public interface IDelayDeleteComponent : IComponent, IEntity
    {
        public DelayDeleteComponent DelayDeleteComponent { get; }
    }

    /// <summary>
    /// 딜레이된 삭제를 처리하기 위한 component
    /// timer가 deletetime에 도달하면 삭제되며 enable = true상태에서만 시간이 체크된다.
    /// </summary>
    [System.Serializable]
    public class DelayDeleteComponent
    {
        public bool isEnable = false;
        public float timer;
        public float deleteTime = 1;
    }

}