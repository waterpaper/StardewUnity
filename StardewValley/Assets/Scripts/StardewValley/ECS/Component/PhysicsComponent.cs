using UnityEngine;

namespace WATP.ECS
{
    public interface IPhysicsComponent : IComponent, ITransformComponent
    {
        public PhysicsComponent PhysicsComponent { get; }
    }

    [System.Serializable]
    public class PhysicsComponent
    {
        /// <summary> ������ ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;

        /// <summary> ���� �̵� �ӵ� /// </summary>
        [SerializeField] public Vector3 velocity;

        /// <summary> ���� /// </summary>
        [SerializeField] public float weight;
    }
}