using System;

namespace WATP.Data
{
    /// <summary>
    /// Json Table data �⺻ ��������
    /// Unity json parser�� list ������ �ٷ� ���� �������� list<T>�� �ѹ� ���μ� �о����
    /// </summary>
    [Serializable]
    public class JsonDefaultForm<T>
    {
        public T data;
    }

    public partial class TableData
    {
        public TableData()
        {
            AddLoadProcess();
        }

        partial void AddLoadProcess();
    }
}


