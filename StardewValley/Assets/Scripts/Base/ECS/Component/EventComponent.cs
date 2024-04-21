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
    /// �ش� �����ӿ� ������ event�� ������ �ִ� component
    /// struct ������ ���� �̺�Ʈ �߻��� EventActionComponent���� ���Ƽ� �߻�(EventSystem)
    /// </summary>
    public struct EventComponent : IComponentData
    {
        public DynamicBuffer<EventBuffer> events;
    }

    /// <summary>
    /// EventComponent�� ������ �̺�Ʈ���� �߻���ų Ŭ����
    /// </summary>
    public class EventActionComponent : IComponentData
    {
        public Action<int> OnEvent;
    }
}