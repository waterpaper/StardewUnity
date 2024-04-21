using Unity.Entities;

namespace WATP.ECS
{
    /// <summary>
    /// �� entity�� ���� �������̽� �Դϴ�.
    /// </summary>
    public interface IEntityBuilder
    {
        /// <summary>
        /// entity�� aspect�� �����մϴ�.
        /// aspect�� �������� �����մϴ�.
        /// </summary>
        IWATPObjectAspect GetObjectAspect();

        /// <summary>
        /// entity�� �����մϴ�.
        /// </summary>
        Entity Build();
    }
}