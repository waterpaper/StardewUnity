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
        /// <summary> ��ǥ ���� </summary>
        [SerializeField] public Action<string> onEvent;
    }
}