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
        /// <summary> ���� ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
    }
}