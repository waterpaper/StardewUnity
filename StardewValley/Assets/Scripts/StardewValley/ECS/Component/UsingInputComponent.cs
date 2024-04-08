using UnityEngine;

namespace WATP.ECS
{
    public interface IUsingInputComponent : IComponent, ITransformComponent, IEventComponent
    {
        public UsingInputComponent UsingInputComponent { get; }
    }

    [System.Serializable]
    public class UsingInputComponent
    {
        [SerializeField] public bool isEnable = true;
        [SerializeField] public bool isAction = false;
        [SerializeField] public float actionTimer;
        [SerializeField] public int actionType;
        [SerializeField] public Vector3 targetPos;
    }
}