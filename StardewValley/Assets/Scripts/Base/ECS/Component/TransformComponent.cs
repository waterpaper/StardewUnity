using UnityEngine;

namespace WATP.ECS
{
    public interface ITransformComponent : IComponent
    {
        public TransformComponent TransformComponent { get; }
    }

    /// <summary>
    /// ���� �⺻���� ������ �ִ� component
    /// </summary>
    [System.Serializable]
    public class TransformComponent
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public Vector3 rotation;
    }
}