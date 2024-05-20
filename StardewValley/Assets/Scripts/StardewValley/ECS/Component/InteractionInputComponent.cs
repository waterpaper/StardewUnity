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

    /// <summary>
    /// entity중 상호작용 입력이 가능한 component
    /// </summary>
    [System.Serializable]
    public class InteractionInputComponent
    {
        /// <summary> 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
    }
}