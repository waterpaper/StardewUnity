using Unity.Entities;

namespace WATP.ECS
{
    public readonly partial struct EventAspect : IAspect
    {
        public readonly Entity entity;

        readonly RefRW<EventComponent> eventComponent;

        public Entity Entity
        {
            get => entity;
        }
        public int Index
        {
            get => entity.Index;
        }
        public EventComponent EventComponent
        {
            get => eventComponent.ValueRO;
            set => eventComponent.ValueRW = value;
        }
    }
}
