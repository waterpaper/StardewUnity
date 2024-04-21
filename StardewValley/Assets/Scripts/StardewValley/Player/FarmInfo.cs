namespace WATP.Player
{
    /// <summary>
    /// 호미질 타일 정보 데이터
    /// </summary>
    public class FarmHoedirtInfo
    {
        public bool watering;
        public float posX;
        public float posY;
    }

    /// <summary>
    /// 농작물 정보 데이터
    /// </summary>
    public class FarmCropsInfo
    {
        public int id;
        public float posX;
        public float posY;
        public int day;
    }
}