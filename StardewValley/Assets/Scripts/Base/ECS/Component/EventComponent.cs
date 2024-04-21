using System;
using Unity.Entities;

namespace WATP.ECS
{
    public enum EventKind
    {
        None = 0,
        Initialize,
        Destroy,
        Idle,
        Move,
        Direction,
        MoveEnd,
        Action,
        ActionEnd,
        Take,
        TakeEnd,
        Day,
        Normal,
        Watering,
        Hit,
        End

    }

    public struct EventBuffer : IBufferElementData
    {
        public int value;
    }

    /// <summary>
    /// 해당 프레임에 설정된 event를 가지고 있는 component
    /// struct 구조로 실제 이벤트 발생은 EventActionComponent에서 몰아서 발생(EventSystem)
    /// </summary>
    public struct EventComponent : IComponentData
    {
        public DynamicBuffer<EventBuffer> events;
    }

    /// <summary>
    /// EventComponent에 설정된 이벤트들을 발생시킬 클래스
    /// </summary>
    public class EventActionComponent : IComponentData
    {
        public Action<int> OnEvent;
    }
}