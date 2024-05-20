using UnityEngine;

namespace WATP.ECS
{
    public interface IMoveInputComponent : IComponent
    {
        public MoveInputComponent MoveInputComponent { get; }
    }

    /// <summary>
    /// entity중 움직임 입력 상태를 처리하기 위한 component
    /// </summary>
    [System.Serializable]
    public class MoveInputComponent
    {
        /// <summary> 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
    }
}