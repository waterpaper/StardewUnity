using UnityEngine;

namespace WATP.ECS
{
    public interface IWarpComponent : IComponent, ITransformComponent
    {
        public WarpComponent WarpComponent { get; }
    }

    /// <summary>
    /// entity중 워프 기능을 처리하기 위한 component
    /// </summary>
    [System.Serializable]
    public class WarpComponent
    {
        /// <summary> 워프 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
    }
}