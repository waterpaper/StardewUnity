namespace WATP.Data
{
    [System.Serializable]
    public class ObjectTableData
    {
        public int Id;
        public string ImagePath;
        public string ImageName;
        public int Index;

        public float Width;
        public float Height;
        public int Hp;

        public float ViewX;
        public float ViewY;

        public bool IsDestroy;
        public int ToolsType;
        public int ToolsLevel;

        public int[] ItemIds;
        public int[] ItemCounts;
    }
}