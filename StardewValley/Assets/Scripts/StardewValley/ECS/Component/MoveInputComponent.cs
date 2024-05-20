using UnityEngine;

namespace WATP.ECS
{
    public interface IMoveInputComponent : IComponent
    {
        public MoveInputComponent MoveInputComponent { get; }
    }

    /// <summary>
    /// entity�� ������ �Է� ���¸� ó���ϱ� ���� component
    /// </summary>
    [System.Serializable]
    public class MoveInputComponent
    {
        /// <summary> ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
    }
}