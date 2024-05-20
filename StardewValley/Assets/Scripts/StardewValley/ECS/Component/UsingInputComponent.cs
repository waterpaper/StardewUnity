using UnityEngine;

namespace WATP.ECS
{
    public interface IUsingInputComponent : IComponent, ITransformComponent, IEventComponent
    {
        public UsingInputComponent UsingInputComponent { get; }
    }

    /// <summary>
    /// entity중 도구 사용 입력 상태를 처리하기 위한 component
    /// </summary>
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