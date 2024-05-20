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
    /// collider를 갖는 component
    /// type에 따라 square, circle이 있으며 충돌 처리가 가능하다.
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