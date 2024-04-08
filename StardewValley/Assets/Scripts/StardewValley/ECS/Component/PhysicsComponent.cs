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
        /// <summary> 움직임 가능여부 </summary>
        [SerializeField] public bool isEnable = true;

        /// <summary> 현재 이동 속도 /// </summary>
        [SerializeField] public Vector3 velocity;

        /// <summary> 무게 /// </summary>
        [SerializeField] public float weight;
    }
}