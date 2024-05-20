namespace WATP.ECS
{
    public interface IHoedirtDataComponent : IComponent, ITransformComponent,IEventComponent
    {
        public HoedirtDataComponent HoedirtDataComponent { get; }
    }

    /// <summary>
    /// 호미질 타일 데이터를 갖는 component
    /// </summary>
    [System.Serializable]
    public class HoedirtDataComponent
    {
        public bool watering = false;
        public bool add = false;
        public bool up = false;
        public bool left = false;
        public bool down = false;
        public bool right = false;
    }

}