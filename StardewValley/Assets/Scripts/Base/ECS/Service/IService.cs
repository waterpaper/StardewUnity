using System.Collections.Generic;

namespace WATP.ECS
{
    /// <summary>
    /// service interface�� ���ϴ� ����� ó���Ѵ�.(���� component�� ��� ����)
    /// ���ϴ� compoenent�� ���� entity�� �̸� ĳ���� �α� ���� add, remove, clear ����
    /// ����� update���� ����
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// ���ϴ� compoenent�� ���� entity�� �̸� ĳ���ϱ� ���� �뵵
        /// </summary>
        void Add(IEntity entity);

        /// <summary>
        /// �̸� ĳ�̵� entity�� �����ϱ� ���� �뵵
        /// </summary>
        void Remove(IEntity entity);

        /// <summary>
        /// �̸� ĳ�̵� entity�� ��ü �����ϱ� ���� �뵵
        /// </summary>
        void Clear();

        void Update(double frameTime);
    }
}