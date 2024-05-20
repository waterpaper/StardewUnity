using UnityEngine;

namespace WATP.ECS
{
    public interface ITargetableComponent : IComponent
    {
        public TargetableComponent TargetableComponent { get; }
    }

    /// <summary>
    /// entity�� target�� ������ component
    /// </summary>
    [System.Serializable]
    public class TargetableComponent
    {
        [SerializeField] public bool isEnable = true;
    }
}