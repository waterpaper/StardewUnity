using UnityEngine;

namespace WATP.ECS
{
    public interface IMoveComponent : IComponent, ITransformComponent, IEventComponent, IPhysicsComponent
    {
        public MoveComponent MoveComponent { get; }
    }

    /// <summary>
    /// entity 중 움직임이 가능한 component
    /// </summary>
    [System.Serializable]
    public class MoveComponent
    {
        /// <summary> 움직임 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
        /// <summary> 목표 /// </summary>
        [SerializeField] public Vector3 targetPos;
        /// <summary> 이전 포지션 /// </summary>
        [SerializeField] public Vector3 beforePos;
        /// <summary> 현재 이동 속도 /// </summary>
        [SerializeField] public float speed;
    }
}