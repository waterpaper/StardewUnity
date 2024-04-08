namespace WATP.ECS
{
    /// <summary>
    /// 각 entity의 빌더 인터페이스 입니다.
    /// </summary>
    public interface IEntityBuilder
    {
        /// <summary>
        /// entity을 생성합니다.
        /// </summary>
        /// <returns></returns>
        IEntity Build();
    }
}