using Unity.Entities;
using Unity.Mathematics;

namespace WATP.ECS
{
    public readonly partial struct MapObjectAspect : IAspect, IWATPObjectAspect
    {
        #region Property
        public readonly Entity entity;

        readonly RefRW<TransformComponent> transformComponent;
        readonly RefRW<EventComponent> eventComponent;
        readonly RefRW<ColliderComponent> colliderComponent;
        readonly RefRW<PhysicsComponent> physicsComponent;
        readonly RefRW<MapObjectDataComponent> mapObjectDataComponent;
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
        public MapObjectDataComponent MapObjectDataComponent
        {
            get => mapObjectDataComponent.ValueRO;
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

        public void Init(float3 pos, int id, int hp)
        {
            transformComponent.ValueRW.position = pos;
            physicsComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.isEnable = true;
            colliderComponent.ValueRW.colliderType = (int)ColliderType.Square;
            colliderComponent.ValueRW.areaWidth = 1;
            colliderComponent.ValueRW.areaHeight = 1;
            deleteComponent.ValueRW.isTimer = true;
            deleteComponent.ValueRW.deleteTime = 0.5f;

            mapObjectDataComponent.ValueRW.id = id;
            mapObjectDataComponent.ValueRW.hp = hp;
            eventComponent.ValueRW.events.Capacity = 5;
        }

        public void OnRef()
        {
            eventComponent.ValueRW.events = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<EventBuffer>(entity);
        }
        #endregion
    }


    #region Builder

    public class MapObjectAspectBuilder : IEntityBuilder
    {
        int id = 0;
        int hp = 0;
        float3 pos = float3.zero;
        EventActionComponent eventActionComponent = new();
        MapObjectAspect mapObjectAspect;

        // 나머지 선택 멤버는 메서드로 설정
        public MapObjectAspectBuilder()
        {
        }

        public MapObjectAspectBuilder SetPos(float3 pos)
        {
            this.pos = pos;
            return this;
        }
        public MapObjectAspectBuilder SetId(int id)
        {
            this.id = id;
            return this;
        }
        public MapObjectAspectBuilder SetHp(int hp)
        {
            this.hp = hp;
            return this;
        }

        public EventActionComponent GetEventSystem()
        {
            return eventActionComponent;
        }

        public IWATPObjectAspect GetObjectAspect()
        {
            return mapObjectAspect;
        }

        public Entity Build()
        {
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ColliderComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PhysicsComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<MapObjectDataComponent>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);

            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

            var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
            eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

            mapObjectAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<MapObjectAspect>(entity);
            mapObjectAspect.Init(pos, id, hp);

            return entity;
        }
    }
    #endregion

}
