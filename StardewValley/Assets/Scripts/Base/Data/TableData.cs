using System;

namespace WATP.Data
{
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


