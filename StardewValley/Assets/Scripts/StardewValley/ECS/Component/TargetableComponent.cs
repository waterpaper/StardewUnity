using UnityEngine;

namespace WATP.ECS
{
    public interface ITargetableComponent : IComponent
    {
        public TargetableComponent TargetableComponent { get; }
    }

    /// <summary>
    /// entity중 target이 가능한 component
    /// </summary>
    [System.Serializable]
    public class TargetableComponent
    {
        [SerializeField] public bool isEnable = true;
    }
}