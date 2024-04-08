using UnityEngine;

namespace WATP.ECS
{
    public interface IWarpComponent : IComponent, ITransformComponent
    {
        public WarpComponent WarpComponent { get; }
    }

    [System.Serializable]
    public class WarpComponent
    {
        /// <summary> 워프 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
    }
}