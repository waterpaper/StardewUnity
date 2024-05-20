using UnityEngine;

namespace WATP.ECS
{
    public enum ColliderType
    {
        None,
        Circle,
        Square,
    }

    public interface IColliderComponent : IComponent, ITransformComponent, IPhysicsComponent
    {
        public ColliderComponent ColliderComponent { get; }
    }

    /// <summary>
    /// collider�� ���� component
    /// type�� ���� square, circle�� ������ �浹 ó���� �����ϴ�.
    /// </summary>
    [System.Serializable]
    public class ColliderComponent
    {
        [SerializeField] public bool isEnable = true;
        [SerializeField] public bool isCheck = false;
        [SerializeField] public ColliderType colliderType = ColliderType.Square;
        [SerializeField] public float areaWidth = 1;
        [SerializeField] public float areaHeight = 1;
    }
}