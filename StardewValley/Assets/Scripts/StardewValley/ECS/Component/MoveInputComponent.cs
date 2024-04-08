using UnityEngine;

namespace WATP.ECS
{
    public interface IMoveInputComponent : IComponent
    {
        public MoveInputComponent MoveInputComponent { get; }
    }

    [System.Serializable]
    public class MoveInputComponent
    {
        /// <summary> 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
    }
}