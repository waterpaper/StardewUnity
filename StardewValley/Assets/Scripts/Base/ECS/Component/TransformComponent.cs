using UnityEngine;

namespace WATP.ECS
{
    public interface ITransformComponent : IComponent
    {
        public TransformComponent TransformComponent { get; }
    }

    /// <summary>
    /// 월드 기본값을 가지고 있는 component
    /// </summary>
    [System.Serializable]
    public class TransformComponent
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 rotation;
    }
}