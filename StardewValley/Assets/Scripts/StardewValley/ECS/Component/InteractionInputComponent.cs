using UnityEngine;

namespace WATP.ECS
{
    public interface IInteractionComponent : IComponent, ITransformComponent
    {
    }

    public interface IInteractionInputComponent : IComponent
    {
        public InteractionInputComponent InteractionInputComponent { get; }
    }

    [System.Serializable]
    public class InteractionInputComponent
    {
        /// <summary> ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
    }
}