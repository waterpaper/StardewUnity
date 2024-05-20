using UnityEngine;

namespace WATP.ECS
{
    public interface IWarpComponent : IComponent, ITransformComponent
    {
        public WarpComponent WarpComponent { get; }
    }

    /// <summary>
    /// entity�� ���� ����� ó���ϱ� ���� component
    /// </summary>
    [System.Serializable]
    public class WarpComponent
    {
        /// <summary> ���� ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
    }
}