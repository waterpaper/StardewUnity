using System;
using UnityEngine;

namespace WATP.ECS
{
    public interface IEventComponent : IComponent
    {
        public EventComponent EventComponent { get; }
    }

    /// <summary>
    /// 해당 프레임에 설정된 event를 가지고 있는 component
    /// </summary>
    [System.Serializable]
    public class EventComponent
    {
        /// <summary> 목표 지점 </summary>
        [SerializeField] public Action<string> onEvent;
    }
}