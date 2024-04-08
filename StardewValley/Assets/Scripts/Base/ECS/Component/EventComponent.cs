using System;
using UnityEngine;

namespace WATP.ECS
{
    public interface IEventComponent : IComponent
    {
        public EventComponent EventComponent { get; }
    }

    [System.Serializable]
    public class EventComponent
    {
        /// <summary> 목표 지점 </summary>
        [SerializeField] public Action<string> onEvent;
    }
}