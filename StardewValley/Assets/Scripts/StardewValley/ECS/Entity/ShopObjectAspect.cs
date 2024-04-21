using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{

    public readonly partial struct ShopObjectAspect : IAspect, IWATPObjectAspect
    {
        public readonly Entity entity;
        readonly RefRW<TransformComponent> transformComponent;
        readonly RefRW<EventComponent> eventComponent;
        readonly RefRW<InteractionComponent> interactionComponent;
        readonly RefRW<ShopObjectComponent> shopObjectComponent;
        readonly RefRW<DeleteComponent> deleteComponent;

        public Entity Entity
        {
            get => entity;
        }
        public int Index
        {
            get => entity.Index;
        }
        public bool DeleteReservation
        {
            get => deleteComponent.ValueRW.isDelate;
            set => deleteComponent.ValueRW.isDelate = value;
        }
        public float3 Position
        {
            get => transformComponent.ValueRO.position;
        }

        public ShopObjectComponent ShopObjectComponent
        {
            get => shopObjectComponent.ValueRO;
        }
        public DeleteComponent DeleteComponent
        {
            get => deleteComponent.ValueRO;
        }

        public void OnInitialize()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Initialize });
        }

        public void OnDestroy()
        {
            eventComponent.ValueRW.events.Add(new EventBuffer() { value = (int)EventKind.Destroy });
        }

        public void OnRef()
        {
            eventComponent.ValueRW.events = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<EventBuffer>(entity);
        }

        public void Init(float3 pos)
        {
            transformComponent.ValueRW.position = pos;
            eventComponent.ValueRW.events.Capacity = 5;
        }
    }

    #region Builder

    public class ShopObjectAspectBuilder : IEntityBuilder
    {
        int id;
        float3 pos = float3.zero;
        EventActionComponent eventActionComponent = new();
        ShopObjectAspect shopObjectAspect;

        // 나머지 선택 멤버는 메서드로 설정
        public ShopObjectAspectBuilder()
        {
        }

        public ShopObjectAspectBuilder SetPos(float3 pos)
        {
            this.pos = pos;
            return this;
        }

        public EventActionComponent GetEventSystem()
        {
            return eventActionComponent;
        }

        public IWATPObjectAspect GetObjectAspect()
        {
            return shopObjectAspect;
        }

        public Entity Build()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<InteractionComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ShopObjectComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

            var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
            eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

            shopObjectAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<ShopObjectAspect>(entity);
            shopObjectAspect.Init(pos);

            return entity;
        }
    }

    #endregion

}
