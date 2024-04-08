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
        /// <summary> ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
    }
}