using UnityEngine;

namespace WATP.ECS
{
    public interface IMoveComponent : IComponent, ITransformComponent, IEventComponent, IPhysicsComponent
    {
        public MoveComponent MoveComponent { get; }
    }

    /// <summary>
    /// entity �� �������� ������ component
    /// </summary>
    [System.Serializable]
    public class MoveComponent
    {
        /// <summary> ������ ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
        /// <summary> ��ǥ /// </summary>
        [SerializeField] public Vector3 targetPos;
        /// <summary> ���� ������ /// </summary>
        [SerializeField] public Vector3 beforePos;
        /// <summary> ���� �̵� �ӵ� /// </summary>
        [SerializeField] public float speed;
    }
}