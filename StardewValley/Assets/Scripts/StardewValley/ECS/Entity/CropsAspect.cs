using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    public readonly partial struct CropsAspect : IAspect, IWATPObjectAspect
    {
        #region Property
        public readonly Entity entity;

        readonly RefRW<TransformComponent> transformComponent;
        readonly RefRW<EventComponent> eventComponent;
        readonly RefRW<ColliderComponent> colliderComponent;
        readonly RefRW<PhysicsComponent> physicsComponent;
        readonly RefRW<InteractionComponent> interactionComponent;
        readonly RefRW<CropsDataComponent> cropsDataComponent;
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

        public CropsDataComponent CropsDataComponent
        {
            get => cropsDataComponent.ValueRO;
        }
        public DeleteComponent DeleteComponent
        {
            get => deleteComponent.ValueRO;
        }

        public int CropsDataDay
        {
            set => cropsDataComponent.ValueRW.day = value;
        }

        public void Init(float3 pos, int id, int day)
        {
            transformComponent.ValueRW.position = pos;
            colliderComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.colliderType = (int)ColliderType.Square;
            colliderComponent.ValueRW.areaWidth = 1;
            colliderComponent.ValueRW.areaHeight = 1;
            deleteComponent.ValueRW.isTimer = true;
            deleteComponent.ValueRW.timer = 0.7f;
            physicsComponent.ValueRW.isEnable = false;

            cropsDataComponent.ValueRW.id = id;
            cropsDataComponent.ValueRW.day = day;
            eventComponent.ValueRW.events.Capacity = 5;
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


        #endregion
    }

    #region Builder

    public class CropsAspectBuilder : IEntityBuilder
    {
        float3 pos = float3.zero;
        int id;
        int day;
        EventActionComponent eventActionComponent = new();
        CropsAspect cropsAspect;

        // 나머지 선택 멤버는 메서드로 설정
        public CropsAspectBuilder()
        {
        }

        public CropsAspectBuilder SetPos(float3 pos)
        {
            this.pos = pos;
            return this;
        }

        public CropsAspectBuilder SetId(int id)
        {
            this.id = id;
            return this;
        }

        public CropsAspectBuilder SetDay(int day)
        {
            this.day = day;
            return this;
        }

        public EventActionComponent GetEventSystem()
        {
            return eventActionComponent;
        }

        public IWATPObjectAspect GetObjectAspect()
        {
            return cropsAspect;
        }

        public Entity Build()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ColliderComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PhysicsComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<InteractionComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<CropsDataComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

            var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
            eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

            cropsAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<CropsAspect>(entity);
            cropsAspect.Init(pos, id, day);

            return entity;
        }
    }
    #endregion

}
