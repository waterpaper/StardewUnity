namespace WATP.Data
{
    [System.Serializable]
    public class CropsTableData
    {
        public int Id;
        public int Type;
        public int Month;
        public int[] Days;
        public int[] Indexs;
        public int LastDay;
        public int ReDay;
        public int ItemId;
    }
}