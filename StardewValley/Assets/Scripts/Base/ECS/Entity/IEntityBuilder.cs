namespace WATP.ECS
{
    /// <summary>
    /// �� entity�� ���� �������̽� �Դϴ�.
    /// </summary>
    public interface IEntityBuilder
    {
        /// <summary>
        /// entity�� �����մϴ�.
        /// </summary>
        /// <returns></returns>
        IEntity Build();
    }
}