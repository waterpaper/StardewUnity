namespace WATP.ECS
{

    public interface ISightComponent : IComponent
    {
        public SightComponent SightComponent { get; }
    }

    [System.Serializable]
    public class SightComponent
    {
        public float value;
    }

}