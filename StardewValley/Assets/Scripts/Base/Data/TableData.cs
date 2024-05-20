using System;

namespace WATP.Data
{
    /// <summary>
    /// Json Table data 기본 구조정의
    /// Unity json parser는 list 구조를 바로 읽지 못함으로 list<T>로 한번 감싸서 읽어야함
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


