namespace WATP.ECS
{
    public interface IDelayDeleteComponent : IComponent, IEntity
    {
        public DelayDeleteComponent DelayDeleteComponent { get; }
    }

    /// <summary>
    /// �����̵� ������ ó���ϱ� ���� component
    /// timer�� deletetime�� �����ϸ� �����Ǹ� enable = true���¿����� �ð��� üũ�ȴ�.
    /// </summary>
    [System.Serializable]
    public class DelayDeleteComponent
    {
        public bool isEnable = false;
        public float timer;
        public float deleteTime = 1;
    }

}